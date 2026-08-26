namespace Plataforma.Domain.Leads;

public enum OrigenLead
{
    CalculadoraObra,
    FormularioContacto,
    LandingPage,
    Referido
}

public enum EstadoLead
{
    Nuevo,
    Contactado,
    Calificado,
    Convertido,
    Descartado,

    // La propiedad de interés del lead se vendió; requiere que un asesor le
    // ofrezca alternativas similares en la misma zona (docs/01-domain-model.md v1.1, §5).
    ContactoPendientePorReasignacion
}

public enum TipoAcabado
{
    Basico,
    Medio,
    Alto
}

public enum TipoProyecto
{
    Vivienda,
    Comercial,
    Industrial
}
