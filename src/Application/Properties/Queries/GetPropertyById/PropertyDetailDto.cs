using Plataforma.Domain.Propiedades;

namespace Plataforma.Application.Properties.Queries.GetPropertyById;

public sealed record PropertyDetailDto(
    Guid Id,
    string Titulo,
    string Descripcion,
    TipoInmueble TipoInmueble,
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
    TipoSuelo TipoSuelo,
    Topografia Topografia,
    decimal? NivelFreaticoMetros,
    EstadoPropiedad Estado,
    bool EsViableConstructivamente,
    IReadOnlyList<string> RestriccionesViabilidad,
    IReadOnlyList<RetiroAmbientalDto> RetirosAmbientales,
    IReadOnlyList<ArchivoMultimediaDto> Multimedia
)
{
    public static PropertyDetailDto DesdeDominio(Propiedad propiedad)
    {
        var viabilidad = propiedad.CalcularViabilidadConstructiva();

        return new PropertyDetailDto(
            propiedad.Id.Value,
            propiedad.Titulo,
            propiedad.Descripcion,
            propiedad.TipoInmueble,
            propiedad.Precio.Monto,
            propiedad.Precio.Moneda,
            propiedad.Ubicacion.Direccion,
            propiedad.Ubicacion.Municipio,
            propiedad.Ubicacion.Departamento,
            propiedad.Ubicacion.Coordenadas?.Latitud,
            propiedad.Ubicacion.Coordenadas?.Longitud,
            propiedad.AreaTerreno.EnMetrosCuadrados(),
            propiedad.AreaConstruida?.EnMetrosCuadrados(),
            propiedad.CaracteristicasTopograficas.PendientePorcentaje,
            propiedad.CaracteristicasTopograficas.TipoSuelo,
            propiedad.CaracteristicasTopograficas.Topografia,
            propiedad.CaracteristicasTopograficas.NivelFreaticoMetros,
            propiedad.Estado,
            viabilidad.EsViable,
            viabilidad.Restricciones,
            propiedad.RetirosAmbientales
                .Select(r => new RetiroAmbientalDto(r.TipoFuente, r.DistanciaMinimaMetros, r.NormativaAplicable))
                .ToList(),
            propiedad.Multimedia
                .OrderBy(m => m.Orden)
                .Select(m => new ArchivoMultimediaDto(m.Id, m.Url, m.Tipo, m.Orden))
                .ToList());
    }
}

public sealed record RetiroAmbientalDto(TipoFuenteRetiro TipoFuente, decimal DistanciaMinimaMetros, string NormativaAplicable);

public sealed record ArchivoMultimediaDto(Guid Id, string Url, TipoMultimedia Tipo, int Orden);
