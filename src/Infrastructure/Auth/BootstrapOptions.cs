namespace Plataforma.Infrastructure.Auth;

// Bindeado desde la sección "Bootstrap" de configuración — deliberadamente
// TODOS opcionales (a diferencia de JwtOptions/AdminApiKeyOptions): estos
// valores solo existen temporalmente, mientras se crea el primer usuario
// Admin, y se retiran de la configuración inmediatamente después (ver
// AdminBootstrapper). Que falten en el arranque normal es el caso esperado,
// no un error de configuración.
public sealed class BootstrapOptions
{
    public const string SectionName = "Bootstrap";

    public string? AdminNombre { get; init; }

    public string? AdminEmail { get; init; }

    public string? AdminPassword { get; init; }
}
