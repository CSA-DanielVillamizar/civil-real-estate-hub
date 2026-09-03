using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Plataforma.Application;
using Plataforma.Domain.Usuarios;
using Plataforma.Infrastructure;
using Plataforma.Infrastructure.Auth;
using Plataforma.WebApi.Endpoints;
using Plataforma.WebApi.ErrorHandling;
using Plataforma.WebApi.Security;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "Frontend";
const string PublicWriteRateLimiterPolicy = "PublicWrite";

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Enums como string en JSON (aprobado en Fase 2) — sin esto, System.Text.Json
// serializa/deserializa enums por su valor numérico, lo que rompe tanto el
// binding del request (ej. "tipoAcabado":"Medio") como las respuestas frente
// a lo que espera api/openapi.yaml y los tipos de frontend/src/types.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Los validadores de Contracts (Fase 2) son un proyecto aparte del que escanea
// AddApplicationServices — se registran aquí porque Contracts es un concepto
// de borde de la Web API, no de Application/Domain.
builder.Services.AddValidatorsFromAssembly(typeof(Plataforma.Contracts.Common.DatosCalculoObraDtoValidator).Assembly);

// Reemplaza el API key administrativo compartido (Fase 3) — ver decisión
// aprobada: JWT propio + roles Admin/AsesorComercial por persona. El resto
// de la configuración (JwtOptions, PasswordHasher, AdminBootstrapper) se
// registra en Infrastructure/DependencyInjection.AddAuth.
var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SigningKey"] ?? string.Empty)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthorizationPolicies.RequiereAdmin, policy => policy.RequireRole(nameof(RolUsuario.Admin)))
    .AddPolicy(
        AuthorizationPolicies.RequiereAsesorOAdmin,
        policy => policy.RequireRole(nameof(RolUsuario.Admin), nameof(RolUsuario.AsesorComercial)));

// Orígenes permitidos configurables (Cors:AllowedOrigins) — en local viene del
// default en appsettings.json (Vite en :5173); en Azure se agrega el dominio
// real de la Static Web App vía App Settings, sin hardcodear una URL de
// despliegue específica en el código fuente.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Manejo de excepciones global (Prompt 6, ítem 2): ApplicationExceptionHandler
// traduce ValidationException a 400 + ProblemDetails; AddProblemDetails()
// cubre cualquier otra excepción no manejada con un 500 + ProblemDetails.
builder.Services.AddExceptionHandler<ApplicationExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Azure App Service (Linux) entrega las requests a través de su propio
// reverse proxy interno: sin esto, HttpContext.Connection.RemoteIpAddress
// siempre sería la IP interna de ese proxy (la misma para todo el mundo),
// lo que volvería inútil el rate limiting por IP de abajo. KnownNetworks/
// KnownProxies se limpian porque la lista de proxies de App Service es
// dinámica — no hay un rango fijo que declarar (mismo patrón recomendado
// por Microsoft para IIS/App Service).
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Rate limiting (gap #6) para los formularios públicos de captura de leads
// — sin CAPTCHA (requeriría credenciales de un servicio externo que el
// negocio todavía no tiene): 10 solicitudes por IP cada 5 minutos alcanza
// para un uso legítimo (varios formularios, algún reintento por validación)
// y frena una ráfaga de spam automatizado. QueueLimit=0 rechaza de
// inmediato en vez de encolar y hacer esperar al resto de solicitantes.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy(PublicWriteRateLimiterPolicy, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "sin-ip",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(5),
                QueueLimit = 0,
            }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/problem+json";
        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                title = "Demasiadas solicitudes. Intenta de nuevo en unos minutos.",
                status = StatusCodes.Status429TooManyRequests,
            },
            cancellationToken);
    };
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapAuthEndpoints();
app.MapLeadsEndpoints(PublicWriteRateLimiterPolicy);
app.MapPropertiesEndpoints();
app.MapBudgetsEndpoints();
app.MapViabilidadAmbientalEndpoints(PublicWriteRateLimiterPolicy);
app.MapObrasEndpoints();
app.MapConfianzaEndpoints();
app.MapTarifasEndpoints();

app.Run();

// Necesario para WebApplicationFactory en pruebas de integración futuras.
public partial class Program;
