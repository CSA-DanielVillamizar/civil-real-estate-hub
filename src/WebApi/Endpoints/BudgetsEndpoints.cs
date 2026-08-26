using FluentValidation;
using Plataforma.Contracts.Common;
using Plataforma.Domain.Leads.Services;
using Plataforma.WebApi.Mapping;
using ApplicationValidationException = Plataforma.Application.Common.Exceptions.ValidationException;

namespace Plataforma.WebApi.Endpoints;

public static class BudgetsEndpoints
{
    public static void MapBudgetsEndpoints(this WebApplication app)
    {
        app.MapPost("/api/budgets/calculate", CalculateAsync)
            .WithName("calculateBudget")
            .WithTags("Budgets");
    }

    // Endpoint "stateless" (ver docs/01-domain-model.md, Fase 2, supuesto confirmado):
    // no pasa por MediatR — inyecta directamente el domain service puro y no
    // persiste nada. Como no hay pipeline de MediatR aquí, la validación se
    // ejecuta explícitamente contra el mismo validador de Fase 2, y cualquier
    // fallo se reporta con la MISMA ValidationException que usa el pipeline,
    // para que ambos caminos terminen en el mismo manejador global de errores.
    private static async Task<IResult> CalculateAsync(
        DatosCalculoObraDto request,
        IValidator<DatosCalculoObraDto> validator,
        CalculadoraDeObraService calculadora,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ApplicationValidationException(validationResult.Errors);

        var datos = request.ToDomain();
        var estimacion = calculadora.Calcular(datos);

        return Results.Ok(estimacion.ToContract());
    }
}
