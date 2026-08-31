namespace Plataforma.Infrastructure.Notifications;

// Bindeado desde la sección "Notifications" de configuración.
public sealed class NotificationsOptions
{
    public const string SectionName = "Notifications";

    // URL de un webhook entrante (Slack, Teams, o un endpoint propio) que
    // recibe una alerta cada vez que se capta un lead nuevo.
    public string? WebhookUrl { get; init; }

    // Endpoint del recurso de Azure Communication Services (autenticación vía
    // Managed Identity — DefaultAzureCredential, sin claves).
    public required string CommunicationServicesEndpoint { get; init; }

    // Dirección del dominio administrado por Azure (formato
    // donotreply@<guid>.azurecomm.net) configurado en Bicep.
    public required string EmailFromAddress { get; init; }

    public string EmailFromDisplayName { get; init; } = "Plataforma Civil e Inmobiliaria";
}
