using Plataforma.Domain.Common;

namespace Plataforma.Domain.Leads.Events;

public sealed record LeadConvertidoEvent(LeadId LeadId) : DomainEvent;
