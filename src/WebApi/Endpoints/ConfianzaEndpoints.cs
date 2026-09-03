using MediatR;
using Plataforma.Application.Confianza.Commands.ActualizarContenidoConfianza;
using Plataforma.Application.Confianza.Commands.CrearContenidoConfianza;
using Plataforma.Application.Confianza.Commands.DespublicarContenidoConfianza;
using Plataforma.Application.Confianza.Commands.PublicarContenidoConfianza;
using Plataforma.Application.Confianza.Queries.ObtenerContenidoConfianzaAdmin;
using Plataforma.Application.Confianza.Queries.ObtenerContenidoConfianzaPublicado;
using Plataforma.Contracts.Confianza;
using Plataforma.WebApi.Mapping;
using Plataforma.WebApi.Security;

namespace Plataforma.WebApi.Endpoints;

// Testimonios de clientes y casos de portafolio (gap #4 — contenido de
// confianza para Consultoría/Interventoría, las 2 líneas de negocio sin
// prueba social hoy). Un solo aggregate/CRUD para ambos tipos — ver
// Domain.Confianza.Enums para el razonamiento.
public static class ConfianzaEndpoints
{
    public static void MapConfianzaEndpoints(this WebApplication app)
    {
        // Público — lo que se muestra en las secciones de Testimonios/Portafolio del sitio.
        app.MapGet("/api/contenido-confianza", GetPublicadoAsync)
            .WithName("getContenidoConfianzaPublicado")
            .WithTags("Confianza");

        // Administrativo: incluye borradores sin publicar.
        app.MapGet("/api/contenido-confianza/admin", GetAdminAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("getContenidoConfianzaAdmin")
            .WithTags("Confianza");

        app.MapPost("/api/contenido-confianza", CrearAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("crearContenidoConfianza")
            .WithTags("Confianza");

        app.MapPut("/api/contenido-confianza/{id:guid}", ActualizarAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("actualizarContenidoConfianza")
            .WithTags("Confianza");

        app.MapPost("/api/contenido-confianza/{id:guid}/publicar", PublicarAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("publicarContenidoConfianza")
            .WithTags("Confianza");

        app.MapPost("/api/contenido-confianza/{id:guid}/despublicar", DespublicarAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("despublicarContenidoConfianza")
            .WithTags("Confianza");
    }

    private static async Task<IResult> GetPublicadoAsync(ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ObtenerContenidoConfianzaPublicadoQuery(), cancellationToken);
        return Results.Ok(result.Select(item => item.ToContract()));
    }

    private static async Task<IResult> GetAdminAsync(ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ObtenerContenidoConfianzaAdminQuery(), cancellationToken);
        return Results.Ok(result.Select(item => item.ToContract()));
    }

    private static async Task<IResult> CrearAsync(CrearContenidoConfianzaRequest request, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(), cancellationToken);
        var response = result.ToContract();
        return Results.Created($"/api/contenido-confianza/{response.Id}", response);
    }

    private static async Task<IResult> ActualizarAsync(
        Guid id, ActualizarContenidoConfianzaRequest request, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> PublicarAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new PublicarContenidoConfianzaCommand(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> DespublicarAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DespublicarContenidoConfianzaCommand(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }
}
