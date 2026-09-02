namespace Plataforma.Contracts.Common;

public enum OrigenLeadDto
{
    CalculadoraObra,
    FormularioContacto,
    LandingPage,
    Referido
}

public enum ServicioDeInteresDto
{
    Inmobiliaria,
    CalculadoraDeObra,
    ConsultoriaYDisenoEstructural,
    InterventoriaYPresupuestos
}

public enum EstadoLeadDto
{
    Nuevo,
    Contactado,
    Calificado,
    Convertido,
    Descartado,
    ContactoPendientePorReasignacion
}

public enum TipoAcabadoDto
{
    Basico,
    Medio,
    Alto
}

public enum TipoProyectoDto
{
    Vivienda,
    Comercial,
    Industrial
}

public enum TipoInmuebleDto
{
    Lote,
    Casa,
    Apartamento,
    Local,
    Bodega,
    Finca
}

public enum EstadoPropiedadDto
{
    Borrador,
    Publicada,
    Reservada,
    Vendida,
    Arrendada,
    Retirada
}

public enum EstadoSolicitudViabilidadDto
{
    Solicitada,
    Pagada,
    Rechazada
}

public enum TipoSueloDto
{
    Arcilloso,
    Arenoso,
    Rocoso,
    Franco,
    Limoso
}

public enum TopografiaDto
{
    Plana,
    Inclinada,
    Irregular
}

public enum TipoFuenteRetiroDto
{
    Rio,
    Quebrada,
    Bosque,
    ViaPrincipal,
    LineaAltaTension
}

public enum TipoMultimediaDto
{
    Foto,
    Plano,
    Render,
    Video
}

public enum UnidadMedidaAreaDto
{
    M2,
    Hectarea
}
