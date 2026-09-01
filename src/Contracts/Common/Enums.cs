namespace Plataforma.Contracts.Common;

public enum OrigenLeadDto
{
    CalculadoraObra,
    FormularioContacto,
    LandingPage,
    Referido
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
