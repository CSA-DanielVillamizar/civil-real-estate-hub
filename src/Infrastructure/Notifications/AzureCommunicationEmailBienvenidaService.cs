using Azure.Communication.Email;
using Microsoft.Extensions.Logging;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads;

namespace Plataforma.Infrastructure.Notifications;

// "Enviar un correo transaccional de bienvenida" vía Azure Communication
// Services. Autenticación por Managed Identity (ver DependencyInjection —
// EmailClient se registra con DefaultAzureCredential, sin API key).
// Resiliencia: EmailClient trae su propia política de reintentos del Azure
// SDK (exponential backoff) — no se envuelve con Polly encima para evitar
// reintentos anidados; el webhook sí lo usa porque ahí no hay SDK propio.
public sealed class AzureCommunicationEmailBienvenidaService : IEmailBienvenidaService
{
    private readonly EmailClient _emailClient;
    private readonly NotificationsOptions _options;
    private readonly ILogger<AzureCommunicationEmailBienvenidaService> _logger;

    public AzureCommunicationEmailBienvenidaService(
        EmailClient emailClient,
        Microsoft.Extensions.Options.IOptions<NotificationsOptions> options,
        ILogger<AzureCommunicationEmailBienvenidaService> logger)
    {
        _emailClient = emailClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task EnviarBienvenidaAsync(Lead lead, CancellationToken cancellationToken)
    {
        var contenido = new EmailContent("¡Gracias por tu interés en tu próximo proyecto!")
        {
            PlainText = $"Hola {lead.Nombre},\n\n" +
                "Recibimos tu solicitud y muy pronto uno de nuestros asesores se pondrá en contacto contigo " +
                "con una cotización detallada para tu proyecto.\n\n" +
                "Plataforma Civil e Inmobiliaria",
            Html = $"<html><body>" +
                $"<p>Hola {lead.Nombre},</p>" +
                "<p>Recibimos tu solicitud y muy pronto uno de nuestros asesores se pondrá en contacto contigo " +
                "con una cotización detallada para tu proyecto.</p>" +
                "<p>Plataforma Civil e Inmobiliaria</p>" +
                "</body></html>",
        };

        var mensaje = new EmailMessage(
            senderAddress: _options.EmailFromAddress,
            recipientAddress: lead.Email.Valor,
            content: contenido);

        // Deliberadamente NO se atrapa la excepción aquí: el command handler
        // (Application) trata webhook + correo como una sola unidad "todo o
        // nada" — si el correo falla, el mensaje completo de la cola se
        // reintenta (ver ProcesarNotificacionLeadCaptadoCommandHandler), así
        // que esta excepción debe propagarse, no absorberse en silencio.
        try
        {
            await _emailClient.SendAsync(Azure.WaitUntil.Started, mensaje, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falló el envío del correo de bienvenida al lead {LeadId} ({Email}).", lead.Id.Value, lead.Email.Valor);
            throw;
        }
    }
}
