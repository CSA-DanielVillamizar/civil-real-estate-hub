using Plataforma.Domain.Common;

namespace Plataforma.Domain.ViabilidadAmbiental.Events;

// Sin handler todavía (a diferencia de LeadCaptadoEvent) — el envío del
// correo con las instrucciones de pago se hace directamente en el command
// handler, después de confirmar la persistencia (ver
// SolicitarViabilidadAmbientalCommandHandler). Este evento queda disponible
// para un futuro consumidor (ej. analítica, auditoría) sin costo adicional.
public sealed record ViabilidadAmbientalSolicitadaEvent(SolicitudViabilidadAmbientalId SolicitudId) : DomainEvent;
