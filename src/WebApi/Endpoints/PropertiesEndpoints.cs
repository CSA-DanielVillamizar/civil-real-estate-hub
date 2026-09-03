using MediatR;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Contracts.Common;
using Plataforma.Contracts.Properties;
using Plataforma.WebApi.Mapping;
using Plataforma.WebApi.Security;
using ApplicationAgregarMultimediaCommand = Plataforma.Application.Properties.Commands.AgregarMultimediaAPropiedad.AgregarMultimediaAPropiedadCommand;

namespace Plataforma.WebApi.Endpoints;

public static class PropertiesEndpoints
{
    public static void MapPropertiesEndpoints(this WebApplication app)
    {
        app.MapGet("/api/properties", GetAsync)
            .WithName("getProperties")
            .WithTags("Properties");

        app.MapGet("/api/properties/{id:guid}", GetByIdAsync)
            .WithName("getPropertyById")
            .WithTags("Properties");

        // Listado administrativo: a diferencia de GET /api/properties
        // (público, siempre filtra Estado=Publicada), este ve cualquier
        // estado — así el panel admin encuentra los borradores pendientes
        // de multimedia/publicación.
        app.MapGet("/api/properties/admin", GetAdminAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("getPropertiesAdmin")
            .WithTags("Properties");

        // Endpoints administrativos — solo Admin (AsesorComercial queda
        // acotado a Leads, ver decisión aprobada de la Fase de autenticación).
        app.MapPost("/api/properties", CreateAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("createProperty")
            .WithTags("Properties");

        app.MapPost("/api/properties/{id:guid}/multimedia", AgregarMultimediaAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .DisableAntiforgery()
            .WithName("agregarMultimediaAPropiedad")
            .WithTags("Properties");

        app.MapPost("/api/properties/{id:guid}/publicar", PublicarAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("publicarPropiedad")
            .WithTags("Properties");

        app.MapPost("/api/properties/{id:guid}/reservar", ReservarAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("reservarPropiedad")
            .WithTags("Properties");

        app.MapPost("/api/properties/{id:guid}/marcar-vendida", MarcarVendidaAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("marcarVendidaPropiedad")
            .WithTags("Properties");

        app.MapPost("/api/properties/{id:guid}/marcar-arrendada", MarcarArrendadaAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("marcarArrendadaPropiedad")
            .WithTags("Properties");

        app.MapPost("/api/properties/{id:guid}/retirar", RetirarAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("retirarPropiedad")
            .WithTags("Properties");

        app.MapPut("/api/properties/{id:guid}", ActualizarDatosBasicosAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("actualizarDatosBasicosPropiedad")
            .WithTags("Properties");
    }

    // La validación de filtros (rangos, paginación) ocurre en el pipeline de
    // MediatR vía GetPropertiesQueryValidator (Fase 3/Application) — no en el
    // binding de ASP.NET Core, que no aplica FluentValidation a query params.
    private static async Task<IResult> GetAsync(
        [AsParameters] GetPropertiesQuery query,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query.ToApplicationQuery(), cancellationToken);
        return Results.Ok(result.ToContract());
    }

    private static async Task<IResult> GetAdminAsync(
        [AsParameters] GetPropertiesAdminQuery query,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query.ToApplicationQuery(), cancellationToken);
        return Results.Ok(result.ToContract());
    }

    private static async Task<IResult> GetByIdAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(id.ToGetByIdQuery(), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> CreateAsync(
        CrearPropiedadRequest request,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(), cancellationToken);
        var response = result.ToContract();

        return Results.Created($"/api/properties/{response.Id}", response);
    }

    // multipart/form-data — el archivo viaja como IFormFile, el tipo
    // (Foto/Plano/Render/Video) como campo de formulario aparte.
    private static async Task<IResult> AgregarMultimediaAsync(
        Guid id,
        IFormFile archivo,
        [FromForm] TipoMultimediaDto tipo,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        await using var stream = archivo.OpenReadStream();

        var command = new ApplicationAgregarMultimediaCommand(id, stream, archivo.FileName, archivo.ContentType, tipo.ToDomain());
        var result = await mediator.Send(command, cancellationToken);

        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> PublicarAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(id.ToPublicarCommand(), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> ReservarAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(id.ToReservarCommand(), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> MarcarVendidaAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(id.ToMarcarVendidaCommand(), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> MarcarArrendadaAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(id.ToMarcarArrendadaCommand(), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> RetirarAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(id.ToRetirarCommand(), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> ActualizarDatosBasicosAsync(
        Guid id, ActualizarDatosBasicosPropiedadRequest request, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }
}
