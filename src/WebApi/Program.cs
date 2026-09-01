using System.Text.Json.Serialization;
using FluentValidation;
using Plataforma.Application;
using Plataforma.Infrastructure;
using Plataforma.WebApi.Endpoints;
using Plataforma.WebApi.ErrorHandling;
using Plataforma.WebApi.Security;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "Frontend";

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

// Fase 3 (SDD): API key del único endpoint administrativo actual — ver
// AdminApiKeyEndpointFilter. ValidateOnStart() evita que la app arranque con
// el endpoint sin protección real por un error de configuración.
builder.Services.AddOptions<AdminApiKeyOptions>()
    .Bind(builder.Configuration.GetSection(AdminApiKeyOptions.SectionName))
    .ValidateOnStart();

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

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);

app.MapLeadsEndpoints();
app.MapPropertiesEndpoints();
app.MapBudgetsEndpoints();
app.MapViabilidadAmbientalEndpoints();

app.Run();

// Necesario para WebApplicationFactory en pruebas de integración futuras.
public partial class Program;
