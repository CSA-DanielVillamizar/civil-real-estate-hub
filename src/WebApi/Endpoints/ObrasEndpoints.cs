using MediatR;
using Plataforma.Application.Obras.Queries.ObtenerProyectoObraPorId;
using Plataforma.Application.Obras.Queries.ObtenerProyectoObraPorToken;
using Plataforma.Application.Obras.Queries.ObtenerProyectosObra;
using Plataforma.Contracts.Obras;
using Plataforma.WebApi.Mapping;
using Plataforma.WebApi.Security;

namespace Plataforma.WebApi.Endpoints;

public static class ObrasEndpoints
{
    public static void MapObrasEndpoints(this WebApplication app)
    {
        // Portal de avance de obra (P3) — endpoints administrativos protegidos
        // con .RequireAuthorization (solo Admin, ver decisión aprobada: el
        // asesor comercial queda acotado a Leads); GetPorTokenAsync es el
        // único público, porque el token ES la credencial de acceso del
        // cliente (ver ProyectoObra.GenerarToken).
        app.MapPost("/api/obras", CrearAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("crearProyectoObra")
            .WithTags("Obras");

        app.MapGet("/api/obras/admin", GetAdminAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("getProyectosObraAdmin")
            .WithTags("Obras");

        app.MapGet("/api/obras/admin/{id:guid}", GetByIdAdminAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("getProyectoObraAdmin")
            .WithTags("Obras");

        app.MapPost("/api/obras/{id:guid}/hitos", AgregarHitoAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("agregarHito")
            .WithTags("Obras");

        app.MapPost("/api/obras/{id:guid}/hitos/{hitoId:guid}/estado", CambiarEstadoHitoAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("cambiarEstadoHito")
            .WithTags("Obras");

        app.MapPost("/api/obras/{id:guid}/hitos/{hitoId:guid}/evidencia", AgregarEvidenciaAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .DisableAntiforgery()
            .WithName("agregarEvidenciaHito")
            .WithTags("Obras");

        app.MapPost("/api/obras/{id:guid}/estado", CambiarEstadoProyectoAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("cambiarEstadoProyecto")
            .WithTags("Obras");

        app.MapGet("/api/obras/por-token/{token}", GetPorTokenAsync)
            .WithName("getProyectoObraPorToken")
            .WithTags("Obras");
    }

    private static async Task<IResult> CrearAsync(CrearProyectoObraRequest request, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(), cancellationToken);
        var response = result.ToContract();
        return Results.Created($"/api/obras/admin/{response.Id}", response);
    }

    private static async Task<IResult> GetAdminAsync(ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ObtenerProyectosObraQuery(), cancellationToken);
        return Results.Ok(result.Select(item => item.ToContract()));
    }

    private static async Task<IResult> GetByIdAdminAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ObtenerProyectoObraPorIdQuery(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> AgregarHitoAsync(
        Guid id, AgregarHitoRequest request, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> CambiarEstadoHitoAsync(
        Guid id, Guid hitoId, CambiarEstadoHitoRequest request, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(id, hitoId), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    // multipart/form-data — mismo patrón que AgregarMultimediaAsync
    // (PropertiesEndpoints): el archivo viaja como IFormFile.
    private static async Task<IResult> AgregarEvidenciaAsync(
        Guid id, Guid hitoId, IFormFile archivo, ISender mediator, CancellationToken cancellationToken)
    {
        await using var stream = archivo.OpenReadStream();
        var command = new Application.Obras.Commands.AgregarEvidenciaHito.AgregarEvidenciaHitoCommand(
            id, hitoId, stream, archivo.FileName, archivo.ContentType);

        var result = await mediator.Send(command, cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> CambiarEstadoProyectoAsync(
        Guid id, CambiarEstadoProyectoRequest request, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> GetPorTokenAsync(string token, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ObtenerProyectoObraPorTokenQuery(token), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }
}
