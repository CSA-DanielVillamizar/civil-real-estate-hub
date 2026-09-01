using Plataforma.Domain.Common;

namespace Plataforma.Domain.ViabilidadAmbiental.Events;

public sealed record ViabilidadAmbientalPagoConfirmadoEvent(SolicitudViabilidadAmbientalId SolicitudId) : DomainEvent;
