using Azure.Communication.Email;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Infrastructure.Messaging;
using Plataforma.Infrastructure.Notifications;
using Plataforma.Infrastructure.Persistence;
using Plataforma.Infrastructure.Persistence.Repositories;
using Plataforma.Infrastructure.Properties;
using Plataforma.Infrastructure.Reporting;
using Plataforma.Infrastructure.ViabilidadAmbiental;
using QuestPDF.Infrastructure;

namespace Plataforma.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "PlataformaDb";

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"No se encontró la cadena de conexión '{ConnectionStringName}' en la configuración.");

        // FinOps (Fase 8): Azure SQL Database (serverless/free tier) en vez de
        // PostgreSQL — evita el costo de un servicio administrado de Postgres
        // en Azure (no existe una capa gratuita equivalente a la de Azure SQL).
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sqlServer =>
                sqlServer.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<ISolicitudViabilidadAmbientalRepository, SolicitudViabilidadAmbientalRepository>();

        // QuestPDF Community License: gratuita para equipos/empresas con
        // ingresos anuales bajo el umbral que publica QuestPDF (a la fecha de
        // esta implementación, USD 1M) — encaja con el objetivo FinOps del
        // MVP. Si el negocio supera ese umbral más adelante, hay que
        // verificar si se necesita licencia comercial.
        QuestPDF.Settings.License = LicenseType.Community;
        RegistrarFuenteEmbebida();
        services.AddSingleton<IPresupuestoPdfGenerator, QuestPdfPresupuestoPdfGenerator>();

        services.AddMensajeriaYNotificaciones(configuration);
        services.AddViabilidadAmbiental(configuration);
        services.AddPropertiesImageStorage(configuration);

        return services;
    }

    // Los contenedores Linux mínimos de App Service (F1) no traen ninguna
    // fuente del sistema — sin esto, QuestPDF falla (o cuelga) al renderizar
    // texto en producción, aunque en local funcione perfecto. Se registra
    // Open Sans embebida (SIL OFL 1.1, ver Reporting/Fonts/OFL.txt) con
    // nombre explícito para no depender de metadatos internos del archivo.
    private static void RegistrarFuenteEmbebida()
    {
        var assembly = typeof(DependencyInjection).Assembly;
        const string resourceName = "Plataforma.Infrastructure.Reporting.Fonts.OpenSans.ttf";

        using var fontStream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"No se encontró el recurso embebido '{resourceName}'.");

        QuestPDF.Drawing.FontManager.RegisterFontWithCustomName(QuestPdfPresupuestoPdfGenerator.FontFamily, fontStream);
    }

    // Fase 2 (SDD — Desacoplamiento Asíncrono + Zero Trust): LeadCaptadoEvent
    // → Azure Storage Queue → BackgroundService en el mismo App Service →
    // webhook comercial + correo (Azure Communication Services). Todas las
    // conexiones usan DefaultAzureCredential (Managed Identity en Azure) —
    // ninguna cadena de conexión con clave/secreto en configuración.
    private static void AddMensajeriaYNotificaciones(this IServiceCollection services, IConfiguration configuration)
    {
        // El binder de configuración de .NET ya exige los miembros "required"
        // de estas clases al enlazar — ValidateOnStart() hace que ese chequeo
        // ocurra al arrancar la app, no en el primer request que los use.
        services.AddOptions<MessagingOptions>()
            .Bind(configuration.GetSection(MessagingOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<NotificationsOptions>()
            .Bind(configuration.GetSection(NotificationsOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MessagingOptions>>().Value;
            var queueServiceClient = new QueueServiceClient(new Uri(options.StorageQueueUri), new DefaultAzureCredential());
            return queueServiceClient.GetQueueClient(options.QueueName);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<NotificationsOptions>>().Value;
            return new EmailClient(new Uri(options.CommunicationServicesEndpoint), new DefaultAzureCredential());
        });

        services.AddScoped<ILeadNotificationQueue, AzureStorageQueueLeadNotificationQueue>();
        services.AddScoped<IEmailBienvenidaService, AzureCommunicationEmailBienvenidaService>();
        services.AddScoped<INotificacionComercialService, WebhookNotificacionComercialService>();

        // Reintentos (SDD — Resiliencia): el webhook es un HTTP genérico sin
        // políticas propias, a diferencia de EmailClient (que ya trae las
        // suyas del Azure SDK) — aquí sí se agrega Polly explícitamente.
        services.AddHttpClient(WebhookNotificacionComercialService.HttpClientName)
            .AddStandardResilienceHandler();

        services.AddHostedService<LeadNotificationQueueProcessor>();
    }

    // Fase 3 (SDD): sin pasarela de pago — solo datos bancarios de referencia
    // (opcionales, ver ViabilidadAmbientalOptions) y reutilización del
    // EmailClient/Communication Services ya registrado arriba.
    private static void AddViabilidadAmbiental(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ViabilidadAmbientalOptions>()
            .Bind(configuration.GetSection(ViabilidadAmbientalOptions.SectionName));

        services.AddScoped<IDatosBancariosProvider, ConfiguracionDatosBancariosProvider>();
        services.AddScoped<IEmailSolicitudViabilidadAmbientalService, AzureCommunicationEmailSolicitudViabilidadAmbientalService>();
    }

    // Reutiliza la Storage Account de Fase 2 (Managed Identity, sin claves) —
    // solo agrega un contenedor Blob nuevo, ver deploy/bicep. El contenedor
    // se crea vía Bicep, no aquí en DI (mismo criterio que EmailClient: sin
    // llamadas de red bloqueantes al arrancar la app).
    private static void AddPropertiesImageStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PropertiesOptions>()
            .Bind(configuration.GetSection(PropertiesOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<PropertiesOptions>>().Value;
            var blobServiceClient = new BlobServiceClient(new Uri(options.BlobServiceUri), new DefaultAzureCredential());
            return blobServiceClient.GetBlobContainerClient(options.ContainerName);
        });

        services.AddScoped<IPropertyImageStorage, AzureBlobPropertyImageStorage>();
    }
}
