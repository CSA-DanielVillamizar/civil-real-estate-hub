using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Plataforma.WebApi.Security;

// Protección mínima para la primera acción administrativa del sistema
// (Fase 3 — ver docs de la fase para la discusión completa: API key
// compartida en vez de un sistema de Identity real, adecuado a la escala
// actual de un solo administrador). Header esperado: X-Admin-Api-Key.
public sealed class AdminApiKeyEndpointFilter : IEndpointFilter
{
    public const string HeaderName = "X-Admin-Api-Key";

    private readonly AdminApiKeyOptions _options;

    public AdminApiKeyEndpointFilter(IOptions<AdminApiKeyOptions> options)
    {
        _options = options.Value;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var apiKeyRecibido = context.HttpContext.Request.Headers[HeaderName].ToString();

        if (!EsValido(apiKeyRecibido))
            return Results.Unauthorized();

        return await next(context);
    }

    // Comparación en tiempo constante — evita filtrar el valor correcto por
    // diferencias de tiempo en una comparación byte a byte convencional.
    private bool EsValido(string apiKeyRecibido)
    {
        var esperado = Encoding.UTF8.GetBytes(_options.ApiKey);
        var recibido = Encoding.UTF8.GetBytes(apiKeyRecibido);

        return recibido.Length == esperado.Length && CryptographicOperations.FixedTimeEquals(recibido, esperado);
    }
}
