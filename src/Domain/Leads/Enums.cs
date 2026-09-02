namespace Plataforma.Domain.Leads;

public enum OrigenLead
{
    CalculadoraObra,
    FormularioContacto,
    LandingPage,
    Referido
}

// Independiente de Origen (que es el canal por el que llegó el lead): esto
// es QUÉ servicio le interesa, información que hoy falta por completo para
// las 2 líneas de negocio sin presencia digital (docs/02-business-case.md
// §3.2/§3.3) — ver Lead.Registrar para las reglas de inferencia automática.
public enum ServicioDeInteres
{
    Inmobiliaria,
    CalculadoraDeObra,
    ConsultoriaYDisenoEstructural,
    InterventoriaYPresupuestos
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
