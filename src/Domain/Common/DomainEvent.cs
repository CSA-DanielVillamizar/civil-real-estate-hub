using MediatR;

namespace Plataforma.Domain.Common;

// ASUNCIÓN: se usa MediatR.INotification como abstracción de despacho de eventos
// de dominio (paquete liviano, sin infraestructura de persistencia). El publish
// real (in-process) se conecta en la capa de Infraestructura/Application (Fase 4).
public interface IDomainEvent : INotification
{
    DateTimeOffset OcurridoEn { get; }
}

public abstract record DomainEvent : IDomainEvent
{
    public DateTimeOffset OcurridoEn { get; } = DateTimeOffset.UtcNow;
}
