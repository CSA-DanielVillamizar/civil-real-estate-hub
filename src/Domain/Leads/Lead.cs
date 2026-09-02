using Plataforma.Domain.Common;
using Plataforma.Domain.Leads.Events;
using Plataforma.Domain.Leads.Exceptions;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Domain.Leads;

public sealed class Lead : AggregateRoot<LeadId>
{
    public string Nombre { get; private set; }
    public Email Email { get; private set; }
    public Telefono Telefono { get; private set; }
    public OrigenLead Origen { get; private set; }
    public EstadoLead Estado { get; private set; }
    public PropiedadId? PropiedadDeInteresId { get; private set; }
    public EstimacionCosto? ResultadoCalculadora { get; private set; }
    public ServicioDeInteres? ServicioDeInteres { get; private set; }
    public string? Mensaje { get; private set; }
    public DateTimeOffset CapturadoEn { get; private set; }

    // Marca de idempotencia (Fase 2 — SDD): el consumidor en background de la
    // cola de notificaciones (alerta al equipo comercial + correo de
    // bienvenida) la revisa antes de procesar, para no reenviar si el mensaje
    // se entrega más de una vez (semántica "at-least-once" de Storage Queues).
    public DateTimeOffset? NotificacionComercialEnviadaEn { get; private set; }

    // Reservado para materialización de EF Core (Fase 4).
    private Lead() { }

    private Lead(
        LeadId id,
        string nombre,
        Email email,
        Telefono telefono,
        OrigenLead origen,
        PropiedadId? propiedadDeInteresId,
        EstimacionCosto? resultadoCalculadora,
        ServicioDeInteres? servicioDeInteres,
        string? mensaje) : base(id)
    {
        Nombre = nombre;
        Email = email;
        Telefono = telefono;
        Origen = origen;
        PropiedadDeInteresId = propiedadDeInteresId;
        ResultadoCalculadora = resultadoCalculadora;
        ServicioDeInteres = servicioDeInteres;
        Mensaje = mensaje;
        Estado = EstadoLead.Nuevo;
        CapturadoEn = DateTimeOffset.UtcNow;
    }

    public static Lead Registrar(
        string nombre,
        Email email,
        Telefono telefono,
        OrigenLead origen,
        PropiedadId? propiedadDeInteresId = null,
        EstimacionCosto? resultadoCalculadora = null,
        ServicioDeInteres? servicioDeInteres = null,
        string? mensaje = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(telefono);

        if (origen == OrigenLead.CalculadoraObra && resultadoCalculadora is null)
            throw new ArgumentException(
                "ResultadoCalculadora es obligatorio cuando el origen es CalculadoraObra.",
                nameof(resultadoCalculadora));

        // Inferencia automática (solo cuando el llamador no lo indica
        // explícito): los flujos existentes (calculadora, interés en una
        // propiedad) ya traen la señal necesaria — los formularios nuevos de
        // Consultoría/Interventoría, que no tienen ninguna señal propia del
        // dominio, sí deben indicarlo explícitamente.
        var servicioResuelto = servicioDeInteres ?? InferirServicioDeInteres(origen, propiedadDeInteresId);
        var mensajeNormalizado = string.IsNullOrWhiteSpace(mensaje) ? null : mensaje.Trim();

        var lead = new Lead(
            LeadId.Nueva(), nombre.Trim(), email, telefono, origen, propiedadDeInteresId, resultadoCalculadora,
            servicioResuelto, mensajeNormalizado);
        lead.AddDomainEvent(new LeadCaptadoEvent(lead.Id, origen));
        return lead;
    }

    private static ServicioDeInteres? InferirServicioDeInteres(OrigenLead origen, PropiedadId? propiedadDeInteresId)
    {
        // Calificado con el namespace completo — dentro de Lead, el nombre
        // simple "ServicioDeInteres" resuelve a la propiedad (mismo nombre
        // que el enum, "problema Color Color" clásico de C#), no al tipo.
        if (origen == OrigenLead.CalculadoraObra)
            return Plataforma.Domain.Leads.ServicioDeInteres.CalculadoraDeObra;

        if (propiedadDeInteresId is not null)
            return Plataforma.Domain.Leads.ServicioDeInteres.Inmobiliaria;

        return null;
    }

    public void MarcarContactado()
    {
        if (Estado != EstadoLead.Nuevo)
            throw new EstadoLeadInvalidoException($"No se puede contactar un lead en estado {Estado}.");

        Estado = EstadoLead.Contactado;
    }

    public void Calificar()
    {
        if (Estado != EstadoLead.Contactado)
            throw new EstadoLeadInvalidoException($"No se puede calificar un lead en estado {Estado}.");

        Estado = EstadoLead.Calificado;
        AddDomainEvent(new LeadCalificadoEvent(Id));
    }

    // Señal de calificación automática: descargar el PDF del presupuesto es
    // evidencia de intención de compra más fuerte que solo usar la
    // calculadora, así que el lead se califica sin pasar por el ciclo manual
    // Contactado → Calificar() (ese sigue existiendo para la calificación
    // hecha por un asesor). Idempotente: si ya estaba Calificado, no repite
    // el evento.
    public void CalificarPorDescargaDePdf()
    {
        if (Estado is EstadoLead.Convertido or EstadoLead.Descartado)
            throw new EstadoLeadInvalidoException($"No se puede calificar un lead en estado {Estado}.");

        if (Estado == EstadoLead.Calificado)
            return;

        Estado = EstadoLead.Calificado;
        AddDomainEvent(new LeadCalificadoEvent(Id));
    }

    // Llamado por el consumidor de la cola (Infrastructure), no dispara un
    // nuevo evento de dominio — es puramente un registro de idempotencia, no
    // un cambio de estado del negocio.
    public void MarcarNotificacionComercialEnviada()
    {
        NotificacionComercialEnviadaEn ??= DateTimeOffset.UtcNow;
    }

    public void Convertir()
    {
        if (Estado != EstadoLead.Calificado)
            throw new EstadoLeadInvalidoException($"No se puede convertir un lead en estado {Estado}.");

        Estado = EstadoLead.Convertido;
        AddDomainEvent(new LeadConvertidoEvent(Id));
    }

    public void Descartar(string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("El motivo de descarte es obligatorio.", nameof(motivo));

        if (Estado is EstadoLead.Convertido)
            throw new EstadoLeadInvalidoException("No se puede descartar un lead ya convertido.");

        Estado = EstadoLead.Descartado;
        AddDomainEvent(new LeadDescartadoEvent(Id, motivo.Trim()));
    }

    // Reacciona a PropiedadVendidaEvent (ver docs/01-domain-model.md v1.1, §5):
    // un prospecto interesado sigue siendo valioso, no se descarta.
    public void RequerirNuevaOferta(PropiedadId propiedadVendidaId, string municipio)
    {
        if (Estado is EstadoLead.Convertido or EstadoLead.Descartado)
            return;

        Estado = EstadoLead.ContactoPendientePorReasignacion;
        AddDomainEvent(new LeadRequiereNuevaOfertaEvent(Id, propiedadVendidaId, municipio));
    }
}
