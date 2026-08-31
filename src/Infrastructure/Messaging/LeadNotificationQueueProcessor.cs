using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plataforma.Application.Common.Messaging;
using Plataforma.Application.Leads.Commands.ProcesarNotificacionLeadCaptado;

namespace Plataforma.Infrastructure.Messaging;

// Consumidor de la cola — "adaptador" técnico entre Azure Storage Queues y el
// caso de uso (ProcesarNotificacionLeadCaptadoCommand, en Application). El
// procesamiento real vive en el command handler; esta clase solo hace polling,
// deserializa, despacha vía MediatR y borra el mensaje si tuvo éxito.
public sealed class LeadNotificationQueueProcessor : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan VisibilityTimeout = TimeSpan.FromMinutes(5);
    private const int MaxMensajesPorLote = 10;

    private readonly QueueClient _queueClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LeadNotificationQueueProcessor> _logger;

    public LeadNotificationQueueProcessor(
        QueueClient queueClient,
        IServiceScopeFactory scopeFactory,
        ILogger<LeadNotificationQueueProcessor> logger)
    {
        _queueClient = queueClient;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcesarLoteAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Deliberadamente dentro del bucle, no antes: si el recurso
                // de Azure todavía no existe (ej. en desarrollo local sin
                // desplegar) o hay un problema transitorio de credenciales,
                // esto se reintenta en el próximo ciclo de polling en vez de
                // tumbar todo el host — por defecto, una excepción sin
                // atrapar en un BackgroundService detiene toda la app.
                _logger.LogError(ex, "Error inesperado consultando la cola de notificaciones de leads.");
            }

            try
            {
                await Task.Delay(PollingInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Apagado normal del host — no es un error.
            }
        }
    }

    private async Task ProcesarLoteAsync(CancellationToken cancellationToken)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var respuesta = await _queueClient.ReceiveMessagesAsync(
            maxMessages: MaxMensajesPorLote,
            visibilityTimeout: VisibilityTimeout,
            cancellationToken: cancellationToken);

        foreach (var mensaje in respuesta.Value)
        {
            await ProcesarMensajeAsync(mensaje, cancellationToken);
        }
    }

    private async Task ProcesarMensajeAsync(QueueMessage mensaje, CancellationToken cancellationToken)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<LeadCaptadoNotificationMessage>(mensaje.MessageText)
                ?? throw new InvalidOperationException("El mensaje de la cola llegó vacío o mal formado.");

            // Scope de DI por mensaje: MediatR y los repositorios (DbContext)
            // están registrados como Scoped, pensados para el ciclo de vida
            // de una petición HTTP — este BackgroundService vive fuera de
            // ese ciclo, así que crea el suyo propio por cada mensaje.
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

            await mediator.Send(new ProcesarNotificacionLeadCaptadoCommand(payload.LeadId), cancellationToken);

            await _queueClient.DeleteMessageAsync(mensaje.MessageId, mensaje.PopReceipt, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // No se borra el mensaje: al vencer VisibilityTimeout reaparece
            // en la cola y se reintenta — sin lógica de reintento adicional.
            _logger.LogError(ex, "Falló el procesamiento del mensaje {MessageId} de la cola de notificaciones de leads.", mensaje.MessageId);
        }
    }
}
