using Plataforma.Domain.Leads.ValueObjects;

namespace Plataforma.Application.Leads.Commands.CreateLead;

public sealed record CreateLeadResult(
    Guid Id,
    string Estado,
    EstimacionCosto? EstimacionCosto
);
