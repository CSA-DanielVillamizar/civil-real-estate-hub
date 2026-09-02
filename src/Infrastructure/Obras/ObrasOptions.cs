namespace Plataforma.Infrastructure.Obras;

// Bindeado desde la sección "Obras" de configuración. Mismo patrón zero-trust
// que PropertiesOptions: solo el endpoint del recurso, la autenticación es
// siempre DefaultAzureCredential (Managed Identity).
public sealed class ObrasOptions
{
    public const string SectionName = "Obras";

    // Misma Storage Account ya usada desde Fase 2/4 (no se aprovisiona una
    // cuenta nueva) — formato https://<cuenta>.blob.core.windows.net/
    public required string BlobServiceUri { get; init; }

    public string ContainerName { get; init; } = "obras-evidencia";
}
