namespace Plataforma.Domain.Propiedades;

public enum TipoInmueble
{
    Lote,
    Casa,
    Apartamento,
    Local,
    Bodega,
    Finca
}

public enum EstadoPropiedad
{
    Borrador,
    Publicada,
    Reservada,
    Vendida,
    Arrendada,
    Retirada
}

public enum TipoMultimedia
{
    Foto,
    Plano,
    Render,
    Video
}

public enum UnidadMedidaArea
{
    M2,
    Hectarea
}

public enum TipoSuelo
{
    Arcilloso,
    Arenoso,
    Rocoso,
    Franco,
    Limoso
}

public enum Topografia
{
    Plana,
    Inclinada,
    Irregular
}

public enum TipoFuenteRetiro
{
    Rio,
    Quebrada,
    Bosque,
    ViaPrincipal,
    LineaAltaTension
}
