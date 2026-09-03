using MediatR;
using Plataforma.Application.Auth.Queries.ObtenerUsuarios;
using Plataforma.Contracts.Auth;
using Plataforma.WebApi.Mapping;
using Plataforma.WebApi.Security;

namespace Plataforma.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        // Público — es el único punto de entrada para obtener un token.
        app.MapPost("/api/auth/login", LoginAsync)
            .WithName("login")
            .WithTags("Auth");

        // Gestión de cuentas de equipo — solo Admin. Reemplaza el sembrado
        // manual por SQL (AdminBootstrapper) para cualquier cuenta después
        // de la primera: el propio Admin crea/desactiva Asesores desde acá.
        app.MapPost("/api/auth/usuarios", CrearUsuarioAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("crearUsuario")
            .WithTags("Auth");

        app.MapGet("/api/auth/usuarios", GetUsuariosAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("getUsuarios")
            .WithTags("Auth");

        app.MapPost("/api/auth/usuarios/{id:guid}/activo", CambiarActivoAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAdmin)
            .WithName("cambiarActivoUsuario")
            .WithTags("Auth");
    }

    private static async Task<IResult> LoginAsync(LoginRequest request, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(), cancellationToken);

        // Mensaje genérico deliberado (ver LoginCommandHandler): no distingue
        // "no existe" de "password incorrecta".
        return result is null
            ? Results.Unauthorized()
            : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> CrearUsuarioAsync(CrearUsuarioRequest request, ISender mediator, CancellationToken cancellationToken)
    {
        // EmailYaRegistradoException (DomainException) se deja propagar — el
        // handler global la traduce a 400 automáticamente.
        var result = await mediator.Send(request.ToCommand(), cancellationToken);
        var response = result.ToContract();
        return Results.Created($"/api/auth/usuarios/{response.Id}", response);
    }

    private static async Task<IResult> GetUsuariosAsync(ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ObtenerUsuariosQuery(), cancellationToken);
        return Results.Ok(result.Select(item => item.ToContract()));
    }

    private static async Task<IResult> CambiarActivoAsync(
        Guid id, CambiarActivoUsuarioRequest request, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }
}
