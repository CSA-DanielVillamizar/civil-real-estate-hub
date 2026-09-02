using MediatR;
using Plataforma.Domain.Obras;

namespace Plataforma.Application.Obras.Commands.CambiarEstadoHito;

public sealed record CambiarEstadoHitoCommand(Guid ProyectoObraId, Guid HitoId, EstadoHito NuevoEstado) : IRequest<CambiarEstadoHitoResult?>;

public sealed record CambiarEstadoHitoResult(Guid HitoId, string Estado);
