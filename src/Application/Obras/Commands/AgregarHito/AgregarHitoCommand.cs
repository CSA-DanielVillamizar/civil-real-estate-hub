using MediatR;

namespace Plataforma.Application.Obras.Commands.AgregarHito;

public sealed record AgregarHitoCommand(
    Guid ProyectoObraId,
    string Nombre,
    string? Descripcion,
    DateOnly? FechaEstimada
) : IRequest<AgregarHitoResult?>;

public sealed record AgregarHitoResult(Guid HitoId, string Nombre, string Estado);
