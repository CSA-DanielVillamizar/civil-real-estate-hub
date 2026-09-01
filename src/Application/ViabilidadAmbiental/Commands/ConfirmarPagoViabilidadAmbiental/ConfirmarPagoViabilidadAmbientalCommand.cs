using MediatR;

namespace Plataforma.Application.ViabilidadAmbiental.Commands.ConfirmarPagoViabilidadAmbiental;

// Acción administrativa (ver AdminApiKeyEndpointFilter en WebApi) — quien
// llega hasta aquí ya pasó el chequeo del API key, así que el command en sí
// no necesita saber nada de autenticación.
public sealed record ConfirmarPagoViabilidadAmbientalCommand(Guid SolicitudId)
    : IRequest<ConfirmarPagoViabilidadAmbientalResult?>;
