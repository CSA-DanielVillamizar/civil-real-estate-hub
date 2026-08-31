using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads;

namespace Plataforma.Infrastructure.Notifications;

// "Alertar al equipo comercial" vía un webhook HTTP genérico — compatible con
// Slack/Teams (incoming webhooks) o un endpoint propio. La resiliencia
// (reintentos, circuit breaker) viene del HttpClient nombrado, configurado en
// DependencyInjection con AddStandardResilienceHandler() (Polly).
public sealed class WebhookNotificacionComercialService : INotificacionComercialService
{
    public const string HttpClientName = "NotificacionComercialWebhook";

    private readonly HttpClient _httpClient;
    private readonly NotificationsOptions _options;
    private readonly ILogger<WebhookNotificacionComercialService> _logger;

    public WebhookNotificacionComercialService(
        IHttpClientFactory httpClientFactory,
        IOptions<NotificationsOptions> options,
        ILogger<WebhookNotificacionComercialService> logger)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientName);
        _options = options.Value;
        _logger = logger;
    }

    public async Task NotificarNuevoLeadAsync(Lead lead, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.WebhookUrl))
        {
            // Sin URL configurada todavía (pendiente de que el equipo
            // comercial defina el canal) — se registra y se continúa, no
            // debe bloquear el resto del procesamiento del lead.
            _logger.LogWarning("No hay Notifications:WebhookUrl configurado — se omite la alerta al equipo comercial para el lead {LeadId}.", lead.Id.Value);
            return;
        }

        var payload = new
        {
            texto = $"🎯 Nuevo lead calificado: {lead.Nombre} ({lead.Email.Valor}, {lead.Telefono}) — origen: {lead.Origen}, municipio: {lead.ResultadoCalculadora?.DatosEntrada.Municipio}",
            leadId = lead.Id.Value,
        };

        var respuesta = await _httpClient.PostAsJsonAsync(_options.WebhookUrl, payload, cancellationToken);
        respuesta.EnsureSuccessStatusCode();
    }
}
