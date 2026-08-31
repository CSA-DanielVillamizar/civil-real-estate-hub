using Plataforma.Application.Common.Messaging;

namespace Plataforma.Application.Common.Interfaces;

// Lado productor: usado por LeadCaptadoEventHandler para desacoplar la
// petición HTTP del trabajo de notificación (Prompt Fase 2 — "dispare una
// tarea en segundo plano de forma asíncrona"). Implementado en Infrastructure
// con Azure Storage Queues.
public interface ILeadNotificationQueue
{
    Task EncolarAsync(LeadCaptadoNotificationMessage mensaje, CancellationToken cancellationToken);
}
