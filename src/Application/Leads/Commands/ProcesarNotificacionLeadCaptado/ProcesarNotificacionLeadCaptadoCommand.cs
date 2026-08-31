using MediatR;

namespace Plataforma.Application.Leads.Commands.ProcesarNotificacionLeadCaptado;

// Despachado por el consumidor de la cola (Infrastructure) al desencolar un
// mensaje — la orquestación real (idempotencia, webhook, correo) vive aquí,
// en Application, siguiendo CQRS/MediatR; el consumidor es solo el
// "adaptador" técnico entre la cola y este caso de uso.
public sealed record ProcesarNotificacionLeadCaptadoCommand(Guid LeadId) : IRequest;
