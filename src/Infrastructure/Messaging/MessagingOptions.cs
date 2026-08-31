namespace Plataforma.Infrastructure.Messaging;

// Bindeado desde la sección "Messaging" de configuración. StorageQueueUri usa
// Managed Identity (DefaultAzureCredential) — sin cadenas de conexión con
// claves de cuenta, por el principio de Zero Trust.
public sealed class MessagingOptions
{
    public const string SectionName = "Messaging";

    public required string StorageQueueUri { get; init; }

    public string QueueName { get; init; } = "lead-notifications";
}
