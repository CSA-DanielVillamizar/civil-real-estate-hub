using MediatR;
using Plataforma.Domain.ViabilidadAmbiental;

namespace Plataforma.Application.ViabilidadAmbiental.Queries.ObtenerSolicitudesViabilidadAmbiental;

// Acción administrativa (misma protección que ConfirmarPagoViabilidadAmbientalCommand
// — ver AuthorizationPolicies en WebApi): lista los datos de contacto de
// los solicitantes, no debe quedar público.
public sealed record ObtenerSolicitudesViabilidadAmbientalQuery(EstadoSolicitudViabilidad? Estado)
    : IRequest<IReadOnlyList<SolicitudViabilidadAmbientalListItem>>;
