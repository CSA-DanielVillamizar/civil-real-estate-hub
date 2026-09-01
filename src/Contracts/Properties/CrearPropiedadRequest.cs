using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Properties;

public sealed record CrearPropiedadRequest(
    string Titulo,
    string Descripcion,
    TipoInmuebleDto TipoInmueble,
    decimal Precio,
    string Moneda,
    string Direccion,
    string Municipio,
    string Departamento,
    decimal? Latitud,
    decimal? Longitud,
    decimal AreaTerrenoValor,
    UnidadMedidaAreaDto AreaTerrenoUnidad,
    decimal? AreaConstruidaValor,
    UnidadMedidaAreaDto? AreaConstruidaUnidad,
    decimal PendientePorcentaje,
    TipoSueloDto TipoSuelo,
    TopografiaDto Topografia,
    decimal? NivelFreaticoMetros,
    IReadOnlyList<RetiroAmbientalRequestDto>? RetirosAmbientales
);

public sealed record RetiroAmbientalRequestDto(TipoFuenteRetiroDto TipoFuente, decimal DistanciaMinimaMetros, string NormativaAplicable);
