using Plataforma.Domain.Common;

namespace Plataforma.Domain.Leads.Events;

public sealed record LeadCaptadoEvent(LeadId LeadId, OrigenLead Origen) : DomainEvent;
