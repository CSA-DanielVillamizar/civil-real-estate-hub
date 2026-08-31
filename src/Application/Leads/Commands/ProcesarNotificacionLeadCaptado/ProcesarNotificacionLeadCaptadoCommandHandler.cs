using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads;

namespace Plataforma.Application.Leads.Commands.ProcesarNotificacionLeadCaptado;

public sealed class ProcesarNotificacionLeadCaptadoCommandHandler : IRequestHandler<ProcesarNotificacionLeadCaptadoCommand>
{
    private readonly ILeadRepository _leadRepository;
    private readonly INotificacionComercialService _notificacionComercialService;
    private readonly IEmailBienvenidaService _emailBienvenidaService;

    public ProcesarNotificacionLeadCaptadoCommandHandler(
        ILeadRepository leadRepository,
        INotificacionComercialService notificacionComercialService,
        IEmailBienvenidaService emailBienvenidaService)
    {
        _leadRepository = leadRepository;
        _notificacionComercialService = notificacionComercialService;
        _emailBienvenidaService = emailBienvenidaService;
    }

    public async Task Handle(ProcesarNotificacionLeadCaptadoCommand request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(new LeadId(request.LeadId), cancellationToken)
            ?? throw new InvalidOperationException(
                $"No se encontró el lead '{request.LeadId}' — el mensaje de la cola llegó antes de que la transacción de creación terminara de confirmar, o el lead fue eliminado.");

        // Idempotencia (SDD — Resiliencia): Storage Queues entrega "al menos
        // una vez"; si este mensaje ya se procesó, no se repite el webhook ni
        // el correo.
        if (lead.NotificacionComercialEnviadaEn is not null)
            return;

        // Secuencial y no en paralelo, a propósito: si el webhook falla, el
        // correo tampoco se envía y el mensaje completo se reintenta desde
        // el principio — un comportamiento "todo o nada" más simple de
        // razonar que la idempotencia granular por cada servicio individual
        // (fuera de alcance para el MVP).
        await _notificacionComercialService.NotificarNuevoLeadAsync(lead, cancellationToken);
        await _emailBienvenidaService.EnviarBienvenidaAsync(lead, cancellationToken);

        lead.MarcarNotificacionComercialEnviada();
        await _leadRepository.UpdateAsync(lead, cancellationToken);
    }
}
