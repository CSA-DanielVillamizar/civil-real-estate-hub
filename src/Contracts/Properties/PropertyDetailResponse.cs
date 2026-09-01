using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Properties;

public sealed record PropertyDetailResponse(
    Guid Id,
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
    decimal AreaTerrenoM2,
    decimal? AreaConstruidaM2,
    decimal PendientePorcentaje,
    TipoSueloDto TipoSuelo,
    TopografiaDto Topografia,
    decimal? NivelFreaticoMetros,
    EstadoPropiedadDto Estado,
    bool EsViableConstructivamente,
    IReadOnlyList<string> RestriccionesViabilidad,
    IReadOnlyList<RetiroAmbientalResponseDto> RetirosAmbientales,
    IReadOnlyList<ArchivoMultimediaResponseDto> Multimedia
);

public sealed record RetiroAmbientalResponseDto(TipoFuenteRetiroDto TipoFuente, decimal DistanciaMinimaMetros, string NormativaAplicable);

public sealed record ArchivoMultimediaResponseDto(Guid Id, string Url, TipoMultimediaDto Tipo, int Orden);
