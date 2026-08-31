using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Infrastructure.Persistence;
using Plataforma.Infrastructure.Persistence.Repositories;
using Plataforma.Infrastructure.Reporting;
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

        // QuestPDF Community License: gratuita para equipos/empresas con
        // ingresos anuales bajo el umbral que publica QuestPDF (a la fecha de
        // esta implementación, USD 1M) — encaja con el objetivo FinOps del
        // MVP. Si el negocio supera ese umbral más adelante, hay que
        // verificar si se necesita licencia comercial.
        QuestPDF.Settings.License = LicenseType.Community;
        services.AddSingleton<IPresupuestoPdfGenerator, QuestPdfPresupuestoPdfGenerator>();

        return services;
    }
}
