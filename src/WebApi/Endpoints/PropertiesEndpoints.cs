using MediatR;
using Plataforma.Contracts.Properties;
using Plataforma.WebApi.Mapping;

namespace Plataforma.WebApi.Endpoints;

public static class PropertiesEndpoints
{
    public static void MapPropertiesEndpoints(this WebApplication app)
    {
        app.MapGet("/api/properties", GetAsync)
            .WithName("getProperties")
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
}
