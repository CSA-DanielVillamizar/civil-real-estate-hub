using MediatR;
using Plataforma.Application.Common.Models;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Application.Properties.Queries.GetProperties;

public sealed record GetPropertiesQuery(
    TipoInmueble? TipoInmueble,
    string? Municipio,
    decimal? PrecioMin,
    decimal? PrecioMax,
    decimal? AreaMin,
    decimal? AreaMax,
    bool? SoloViablesConstructivamente,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<PropertyDto>>;
