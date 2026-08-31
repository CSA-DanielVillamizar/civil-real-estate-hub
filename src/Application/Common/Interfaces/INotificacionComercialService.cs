using Plataforma.Domain.Leads;

namespace Plataforma.Application.Common.Interfaces;

// "Alertar al equipo comercial" (Prompt Fase 2) — implementado en
// Infrastructure vía webhook (URL configurable: Slack/Teams/endpoint propio).
public interface INotificacionComercialService
{
    Task NotificarNuevoLeadAsync(Lead lead, CancellationToken cancellationToken);
}
