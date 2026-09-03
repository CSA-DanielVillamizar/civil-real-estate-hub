using MediatR;
using Plataforma.Application.Tarifas.Commands.ActualizarPaqueteTarifa;
using Plataforma.Application.Tarifas.Commands.CrearPaqueteTarifa;
using Plataforma.Application.Tarifas.Commands.DespublicarPaqueteTarifa;
using Plataforma.Application.Tarifas.Commands.PublicarPaqueteTarifa;
using Plataforma.Application.Tarifas.Queries.ObtenerPaquetesTarifaAdmin;
using Plataforma.Application.Tarifas.Queries.ObtenerPaquetesTarifaPublicados;
using Plataforma.Contracts.Tarifas;
using Plataforma.WebApi.Mapping;
using Plataforma.WebApi.Security;

namespace Plataforma.WebApi.Endpoints;

// Transparencia de precios (gap #5) para consultoría estructural e
// interventoría — mismo patrón CRUD-con-publicación que ConfianzaEndpoints.
public static class TarifasEndpoints
{
    public static void MapTarifasEndpoints(this WebApplication app)
    {
        // Público — lo que se muestra en las secciones de Consultoría/Interventoría del sitio.
        app.MapGet("/api/paquetes-tarifa", GetPublicadosAsync)
            .WithName("getPaquetesTarifaPublicados")
            .WithTags("Tarifas");

        // Administrativo: incluye borradores sin publicar.
        app.MapGet("/api/paquetes-tarifa/admin", GetAdminAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("getPaquetesTarifaAdmin")
            .WithTags("Tarifas");

        app.MapPost("/api/paquetes-tarifa", CrearAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("crearPaqueteTarifa")
            .WithTags("Tarifas");

        app.MapPut("/api/paquetes-tarifa/{id:guid}", ActualizarAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("actualizarPaqueteTarifa")
            .WithTags("Tarifas");

        app.MapPost("/api/paquetes-tarifa/{id:guid}/publicar", PublicarAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("publicarPaqueteTarifa")
            .WithTags("Tarifas");

        app.MapPost("/api/paquetes-tarifa/{id:guid}/despublicar", DespublicarAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("despublicarPaqueteTarifa")
            .WithTags("Tarifas");
    }

    private static async Task<IResult> GetPublicadosAsync(ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ObtenerPaquetesTarifaPublicadosQuery(), cancellationToken);
        return Results.Ok(result.Select(item => item.ToContract()));
    }

    private static async Task<IResult> GetAdminAsync(ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ObtenerPaquetesTarifaAdminQuery(), cancellationToken);
        return Results.Ok(result.Select(item => item.ToContract()));
    }

    private static async Task<IResult> CrearAsync(CrearPaqueteTarifaRequest request, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(), cancellationToken);
        var response = result.ToContract();
        return Results.Created($"/api/paquetes-tarifa/{response.Id}", response);
    }

    private static async Task<IResult> ActualizarAsync(
        Guid id, ActualizarPaqueteTarifaRequest request, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> PublicarAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new PublicarPaqueteTarifaCommand(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> DespublicarAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DespublicarPaqueteTarifaCommand(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }
}
