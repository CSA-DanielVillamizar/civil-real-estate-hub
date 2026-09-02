using MediatR;
using Plataforma.Domain.Leads;

namespace Plataforma.Application.Leads.Queries.ObtenerLeads;

// Acción administrativa (protegida con .RequireAuthorization — ver
// AuthorizationPolicies en WebApi): expone datos de contacto de los leads,
// no debe ser público.
public sealed record ObtenerLeadsQuery(EstadoLead? Estado) : IRequest<IReadOnlyList<LeadListItem>>;
