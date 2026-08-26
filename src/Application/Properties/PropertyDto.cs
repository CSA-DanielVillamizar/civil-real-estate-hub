using Plataforma.Domain.Propiedades;

namespace Plataforma.Application.Properties;

public sealed record PropertyDto(
    Guid Id,
    string Titulo,
    TipoInmueble TipoInmueble,
    decimal Precio,
    string Moneda,
    string Municipio,
    string Departamento,
    decimal AreaTerrenoM2,
    decimal? AreaConstruidaM2,
    EstadoPropiedad Estado,
    string? FotoPrincipalUrl,
    bool EsViableConstructivamente
)
{
    public static PropertyDto DesdeDominio(Propiedad propiedad)
    {
        var viabilidad = propiedad.CalcularViabilidadConstructiva();
        var fotoPrincipal = propiedad.Multimedia
            .Where(m => m.Tipo == TipoMultimedia.Foto)
            .OrderBy(m => m.Orden)
            .FirstOrDefault();

        return new PropertyDto(
            propiedad.Id.Value,
            propiedad.Titulo,
            propiedad.TipoInmueble,
            propiedad.Precio.Monto,
            propiedad.Precio.Moneda,
            propiedad.Ubicacion.Municipio,
            propiedad.Ubicacion.Departamento,
            propiedad.AreaTerreno.EnMetrosCuadrados(),
            propiedad.AreaConstruida?.EnMetrosCuadrados(),
            propiedad.Estado,
            fotoPrincipal?.Url,
            viabilidad.EsViable);
    }
}
