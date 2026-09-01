using Azure.Communication.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plataforma.Application.Common;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.ViabilidadAmbiental;
using Plataforma.Infrastructure.Notifications;

namespace Plataforma.Infrastructure.ViabilidadAmbiental;

// Reutiliza el mismo EmailClient/recurso de Azure Communication Services de
// Fase 2 (Notifications) — no se aprovisiona infraestructura nueva para este
// correo, solo un remitente distinto en el contenido.
public sealed class AzureCommunicationEmailSolicitudViabilidadAmbientalService : IEmailSolicitudViabilidadAmbientalService
{
    private readonly EmailClient _emailClient;
    private readonly NotificationsOptions _notificationsOptions;
    private readonly ILogger<AzureCommunicationEmailSolicitudViabilidadAmbientalService> _logger;

    public AzureCommunicationEmailSolicitudViabilidadAmbientalService(
        EmailClient emailClient,
        IOptions<NotificationsOptions> notificationsOptions,
        ILogger<AzureCommunicationEmailSolicitudViabilidadAmbientalService> logger)
    {
        _emailClient = emailClient;
        _notificationsOptions = notificationsOptions.Value;
        _logger = logger;
    }

    public async Task EnviarInstruccionesPagoAsync(
        SolicitudViabilidadAmbiental solicitud, DatosBancarios datosBancarios, CancellationToken cancellationToken)
    {
        var monto = $"{solicitud.Monto.Monto:N0} {solicitud.Monto.Moneda}";

        var contenido = new EmailContent($"Instrucciones de pago — Estudio de Viabilidad Ambiental #{solicitud.Id.Value}")
        {
            PlainText = $"Hola {solicitud.Solicitante.Nombre},\n\n" +
                $"Recibimos tu solicitud de estudio de viabilidad ambiental. El valor del estudio es {monto}.\n\n" +
                $"Puedes transferir a:\n" +
                $"Banco: {datosBancarios.Banco}\n" +
                $"Tipo de cuenta: {datosBancarios.TipoCuenta}\n" +
                $"Número de cuenta: {datosBancarios.NumeroCuenta}\n" +
                $"Titular: {datosBancarios.TitularCuenta}\n\n" +
                "Una vez confirmemos el pago, un consultor se pondrá en contacto contigo.\n\n" +
                "Plataforma Civil e Inmobiliaria",
            Html = $"<html><body>" +
                $"<p>Hola {solicitud.Solicitante.Nombre},</p>" +
                $"<p>Recibimos tu solicitud de estudio de viabilidad ambiental. El valor del estudio es <strong>{monto}</strong>.</p>" +
                "<p>Puedes transferir a:</p>" +
                "<ul>" +
                $"<li>Banco: {datosBancarios.Banco}</li>" +
                $"<li>Tipo de cuenta: {datosBancarios.TipoCuenta}</li>" +
                $"<li>Número de cuenta: {datosBancarios.NumeroCuenta}</li>" +
                $"<li>Titular: {datosBancarios.TitularCuenta}</li>" +
                "</ul>" +
                "<p>Una vez confirmemos el pago, un consultor se pondrá en contacto contigo.</p>" +
                "<p>Plataforma Civil e Inmobiliaria</p>" +
                "</body></html>",
        };

        var mensaje = new EmailMessage(
            senderAddress: _notificationsOptions.EmailFromAddress,
            recipientAddress: solicitud.Solicitante.Email.Valor,
            content: contenido);

        // Mismo criterio que AzureCommunicationEmailBienvenidaService: se
        // registra y se relanza — es el llamador quien decide si absorbe el
        // fallo. Aquí lo absorbe SolicitarViabilidadAmbientalCommandHandler
        // directamente (try/catch síncrono), no un consumidor de cola.
        try
        {
            await _emailClient.SendAsync(Azure.WaitUntil.Started, mensaje, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Falló el envío de instrucciones de pago para la solicitud {SolicitudId} ({Email}).",
                solicitud.Id.Value, solicitud.Solicitante.Email.Valor);
            throw;
        }
    }
}
