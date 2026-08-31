using MediatR;
using Microsoft.Extensions.Logging;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Common.Messaging;
using Plataforma.Domain.Leads.Events;

namespace Plataforma.Application.Leads.Events;

// Domain Event Handler (Prompt Fase 2): reacciona a LeadCaptadoEvent
// encolando un mensaje. Corre DENTRO de la misma transacción que persiste el
// Lead (ver ApplicationDbContext.SaveChangesAsync) — pero a propósito NO deja
// que un fallo del encolado (Azure Storage Queue caído, credenciales,
// latencia) revierta esa transacción: la captura del lead es la operación
// primaria del negocio, la notificación es secundaria. Justo el problema que
// Fase 2 pidió evitar ("no bloquear los comandos HTTP con llamadas
// externas") — si esto propagara la excepción, un servicio de notificación
// caído tumbaría la captación de leads.
public sealed class LeadCaptadoEventHandler : INotificationHandler<LeadCaptadoEvent>
{
    private readonly ILeadNotificationQueue _notificationQueue;
    private readonly ILogger<LeadCaptadoEventHandler> _logger;

    public LeadCaptadoEventHandler(ILeadNotificationQueue notificationQueue, ILogger<LeadCaptadoEventHandler> logger)
    {
        _notificationQueue = notificationQueue;
        _logger = logger;
    }

    public async Task Handle(LeadCaptadoEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var mensaje = new LeadCaptadoNotificationMessage(notification.LeadId.Value);
            await _notificationQueue.EncolarAsync(mensaje, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Se pierde esta notificación puntual, pero el lead queda
            // capturado correctamente — la alternativa (relanzar) es peor
            // para el negocio: perder el lead completo por un problema
            // ajeno a la captación.
            _logger.LogError(ex, "No se pudo encolar la notificación para el lead {LeadId} — el lead sí quedó registrado.", notification.LeadId.Value);
        }
    }
}
