namespace Plataforma.Infrastructure.ViabilidadAmbiental;

// Bindeado desde la sección "ViabilidadAmbiental" de configuración. Todos los
// campos son opcionales con default vacío (a diferencia de MessagingOptions/
// NotificationsOptions, que exigen sus campos requeridos con
// ValidateOnStart): el usuario confirmó no tener aún estos datos (banco/QR),
// y el flujo debe seguir siendo desplegable — el frontend decide cómo
// mostrar "pendiente de publicar" cuando llegan vacíos.
public sealed class ViabilidadAmbientalOptions
{
    public const string SectionName = "ViabilidadAmbiental";

    public string Banco { get; init; } = string.Empty;
    public string TipoCuenta { get; init; } = string.Empty;
    public string NumeroCuenta { get; init; } = string.Empty;
    public string TitularCuenta { get; init; } = string.Empty;
    public string QrImageUrl { get; init; } = string.Empty;
}
