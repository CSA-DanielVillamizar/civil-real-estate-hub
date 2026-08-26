using Plataforma.Domain.Propiedades;

namespace Plataforma.Application.Common.Interfaces;

public sealed record PropertyFilter(
    TipoInmueble? TipoInmueble,
    string? Municipio,
    decimal? PrecioMin,
    decimal? PrecioMax,
    decimal? AreaMin,
    decimal? AreaMax,
    bool? SoloViablesConstructivamente
);
