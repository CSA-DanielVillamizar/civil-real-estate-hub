namespace Plataforma.Infrastructure.Auth;

// Bindeado desde la sección "Jwt" de configuración. Requerido, igual que
// AdminApiKeyOptions antes: la autenticación no debe poder quedar
// accidentalmente sin protección por falta de configuración —
// ValidateOnStart() lo garantiza (ver DependencyInjection.AddAuth).
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    // Secreto simétrico (HMAC-SHA256) — mínimo 32 caracteres. Se genera una
    // sola vez y se guarda como App Setting en Azure, igual que se hizo con
    // Admin:ApiKey en la Fase 3 (ver docs de esa fase).
    public required string SigningKey { get; init; }

    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public int ExpiracionEnMinutos { get; init; } = 8 * 60;
}
