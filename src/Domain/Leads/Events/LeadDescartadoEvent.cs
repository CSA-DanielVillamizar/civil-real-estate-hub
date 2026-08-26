using Plataforma.Domain.Common;

namespace Plataforma.Domain.Leads.Events;

public sealed record LeadDescartadoEvent(LeadId LeadId, string Motivo) : DomainEvent;
