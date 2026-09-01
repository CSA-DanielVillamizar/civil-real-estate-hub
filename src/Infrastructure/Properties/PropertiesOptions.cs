namespace Plataforma.Infrastructure.Properties;

// Bindeado desde la sección "Properties" de configuración. Mismo patrón
// zero-trust que Messaging/Notifications: solo el endpoint del recurso, la
// autenticación es siempre DefaultAzureCredential (Managed Identity).
public sealed class PropertiesOptions
{
    public const string SectionName = "Properties";

    // Endpoint Blob de la MISMA Storage Account ya usada para las colas de
    // Fase 2 (no se aprovisiona una cuenta nueva) — formato
    // https://<cuenta>.blob.core.windows.net/
    public required string BlobServiceUri { get; init; }

    public string ContainerName { get; init; } = "propiedades-multimedia";
}
