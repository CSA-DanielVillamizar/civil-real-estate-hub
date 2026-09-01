using Plataforma.Domain.Common;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.SharedKernel;
using Plataforma.Domain.ViabilidadAmbiental.Events;
using Plataforma.Domain.ViabilidadAmbiental.Exceptions;
using Plataforma.Domain.ViabilidadAmbiental.ValueObjects;

namespace Plataforma.Domain.ViabilidadAmbiental;

public sealed class SolicitudViabilidadAmbiental : AggregateRoot<SolicitudViabilidadAmbientalId>
{
    public DatosSolicitante Solicitante { get; private set; }
    public PropiedadId? PropiedadId { get; private set; }
    public UbicacionLote? UbicacionLote { get; private set; }
    public Dinero Monto { get; private set; }
    public EstadoSolicitudViabilidad Estado { get; private set; }
    public DateTimeOffset SolicitadaEn { get; private set; }
    public DateTimeOffset? PagoConfirmadoEn { get; private set; }

    // Reservado para materialización de EF Core.
    private SolicitudViabilidadAmbiental() { }

    private SolicitudViabilidadAmbiental(
        SolicitudViabilidadAmbientalId id,
        DatosSolicitante solicitante,
        PropiedadId? propiedadId,
        UbicacionLote? ubicacionLote,
        Dinero monto) : base(id)
    {
        Solicitante = solicitante;
        PropiedadId = propiedadId;
        UbicacionLote = ubicacionLote;
        Monto = monto;
        Estado = EstadoSolicitudViabilidad.Solicitada;
        SolicitadaEn = DateTimeOffset.UtcNow;
    }

    public static SolicitudViabilidadAmbiental Solicitar(
        DatosSolicitante solicitante,
        Dinero monto,
        PropiedadId? propiedadId = null,
        UbicacionLote? ubicacionLote = null)
    {
        ArgumentNullException.ThrowIfNull(solicitante);
        ArgumentNullException.ThrowIfNull(monto);

        // Invariante del agregado: el lote se identifica de una de dos formas
        // (ya está en el catálogo, o el cliente describe dónde está), nunca
        // de ambas ni de ninguna.
        if (propiedadId is null && ubicacionLote is null)
            throw new ArgumentException(
                "Debe indicarse una propiedad existente (propiedadId) o la ubicación del lote (ubicacionLote).");

        var solicitud = new SolicitudViabilidadAmbiental(
            SolicitudViabilidadAmbientalId.Nueva(), solicitante, propiedadId, ubicacionLote, monto);

        solicitud.AddDomainEvent(new ViabilidadAmbientalSolicitadaEvent(solicitud.Id));
        return solicitud;
    }

    // Llamada por el administrador tras verificar manualmente la transferencia
    // (Fase 3 — sin pasarela de pago, ver docs de la fase). Idempotente en el
    // sentido de que un segundo intento sobre una solicitud ya pagada falla
    // explícitamente en vez de reenviar el evento en silencio — a diferencia
    // de Lead.MarcarNotificacionComercialEnviada, aquí SÍ queremos que un
    // reintento accidental del administrador sea visible como error.
    public void ConfirmarPago()
    {
        if (Estado != EstadoSolicitudViabilidad.Solicitada)
            throw new EstadoSolicitudViabilidadInvalidoException(
                $"No se puede confirmar el pago de una solicitud en estado {Estado}.");

        Estado = EstadoSolicitudViabilidad.Pagada;
        PagoConfirmadoEn = DateTimeOffset.UtcNow;
        AddDomainEvent(new ViabilidadAmbientalPagoConfirmadoEvent(Id));
    }

    // Salida para cuando el administrador no logra verificar la transferencia
    // (monto no coincide, nunca llegó, etc.). Sin endpoint HTTP expuesto
    // todavía en Fase 3 — se agrega cuando haya un flujo real para usarla.
    public void Rechazar(string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("El motivo de rechazo es obligatorio.", nameof(motivo));

        if (Estado != EstadoSolicitudViabilidad.Solicitada)
            throw new EstadoSolicitudViabilidadInvalidoException(
                $"No se puede rechazar una solicitud en estado {Estado}.");

        Estado = EstadoSolicitudViabilidad.Rechazada;
    }
}
