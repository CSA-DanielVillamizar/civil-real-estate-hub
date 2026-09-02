using MediatR;
using Plataforma.Domain.Obras;

namespace Plataforma.Application.Obras.Commands.CambiarEstadoProyecto;

public sealed record CambiarEstadoProyectoCommand(Guid ProyectoObraId, EstadoProyecto NuevoEstado) : IRequest<CambiarEstadoProyectoResult?>;

public sealed record CambiarEstadoProyectoResult(Guid Id, string Estado);
