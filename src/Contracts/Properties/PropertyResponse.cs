using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Properties;

public sealed record PropertyResponse(
    Guid Id,
    string Titulo,
    TipoInmuebleDto TipoInmueble,
    decimal Precio,
    string Moneda,
    string Municipio,
    string Departamento,
    decimal AreaTerrenoM2,
    decimal? AreaConstruidaM2,
    EstadoPropiedadDto Estado,
    string? FotoPrincipalUrl,
    bool EsViableConstructivamente
);
