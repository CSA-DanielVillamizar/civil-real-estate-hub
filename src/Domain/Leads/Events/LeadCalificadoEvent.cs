using Plataforma.Domain.Common;

namespace Plataforma.Domain.Leads.Events;

public sealed record LeadCalificadoEvent(LeadId LeadId) : DomainEvent;
