using MediatR;
using Plataforma.Contracts.Auth;
using Plataforma.WebApi.Mapping;

namespace Plataforma.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        // Público — es el único punto de entrada para obtener un token.
        app.MapPost("/api/auth/login", LoginAsync)
            .WithName("login")
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
}
