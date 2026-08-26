using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Properties;

public sealed record GetPropertiesQuery(
    TipoInmuebleDto? TipoInmueble,
    string? Municipio,
    decimal? PrecioMin,
    decimal? PrecioMax,
    decimal? AreaMin,
    decimal? AreaMax,
    bool? SoloViablesConstructivamente,
    int Page = 1,
    int PageSize = 20
);
