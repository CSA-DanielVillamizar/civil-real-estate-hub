namespace Plataforma.WebApi.Security;

// Bindeado desde la sección "Admin" de configuración. Requerido (a diferencia
// de ViabilidadAmbientalOptions): la primera acción administrativa real del
// sistema (confirmar pago) no debe poder quedar accidentalmente sin proteger
// por falta de configuración — ValidateOnStart() lo garantiza (ver Program.cs).
public sealed class AdminApiKeyOptions
{
    public const string SectionName = "Admin";

    public required string ApiKey { get; init; }
}
