using MediatR;
using Plataforma.Domain.Leads;

namespace Plataforma.Application.Leads.Queries.ObtenerLeads;

// Acción administrativa (mismo AdminApiKeyEndpointFilter que ViabilidadAmbiental
// y Properties — un solo mecanismo de protección en todo el sistema): expone
// datos de contacto de los leads, no debe ser público.
public sealed record ObtenerLeadsQuery(EstadoLead? Estado) : IRequest<IReadOnlyList<LeadListItem>>;
