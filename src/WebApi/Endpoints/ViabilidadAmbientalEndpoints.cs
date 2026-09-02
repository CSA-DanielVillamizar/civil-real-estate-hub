using MediatR;
using Plataforma.Contracts.ViabilidadAmbiental;
using Plataforma.WebApi.Mapping;
using Plataforma.WebApi.Security;

namespace Plataforma.WebApi.Endpoints;

public static class ViabilidadAmbientalEndpoints
{
    public static void MapViabilidadAmbientalEndpoints(this WebApplication app)
    {
        app.MapPost("/api/viabilidad-ambiental/solicitudes", SolicitarAsync)
            .WithName("solicitarViabilidadAmbiental")
            .WithTags("ViabilidadAmbiental");

        // Endpoints administrativos — solo Admin. GetAsync expone datos de
        // contacto de los solicitantes, por eso también queda protegido (no
        // solo la escritura).
        app.MapGet("/api/viabilidad-ambiental/solicitudes", GetAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("getSolicitudesViabilidadAmbiental")
            .WithTags("ViabilidadAmbiental");

        app.MapPost("/api/viabilidad-ambiental/solicitudes/{id:guid}/confirmar-pago", ConfirmarPagoAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("confirmarPagoViabilidadAmbiental")
            .WithTags("ViabilidadAmbiental");
    }

    private static async Task<IResult> GetAsync(
        [AsParameters] GetSolicitudesViabilidadAmbientalQuery query,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query.ToApplicationQuery(), cancellationToken);
        return Results.Ok(result.Select(item => item.ToContract()));
    }

    private static async Task<IResult> SolicitarAsync(
        SolicitarViabilidadAmbientalRequest request,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(), cancellationToken);
        var response = result.ToContract();

        return Results.Created($"/api/viabilidad-ambiental/solicitudes/{response.Id}", response);
    }

    private static async Task<IResult> ConfirmarPagoAsync(
        Guid id,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(id.ToConfirmarPagoCommand(), cancellationToken);

        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }
}
