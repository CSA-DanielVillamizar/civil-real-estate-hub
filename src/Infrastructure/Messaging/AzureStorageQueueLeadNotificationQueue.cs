using System.Text.Json;
using Azure.Storage.Queues;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Common.Messaging;

namespace Plataforma.Infrastructure.Messaging;

// Lado productor — usado por LeadCaptadoEventHandler (Application). Solo
// serializa y encola; no sabe nada de webhooks ni correos.
public sealed class AzureStorageQueueLeadNotificationQueue : ILeadNotificationQueue
{
    private readonly QueueClient _queueClient;

    public AzureStorageQueueLeadNotificationQueue(QueueClient queueClient)
    {
        _queueClient = queueClient;
    }

    public async Task EncolarAsync(LeadCaptadoNotificationMessage mensaje, CancellationToken cancellationToken)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var payload = JsonSerializer.Serialize(mensaje);
        await _queueClient.SendMessageAsync(payload, cancellationToken);
    }
}
