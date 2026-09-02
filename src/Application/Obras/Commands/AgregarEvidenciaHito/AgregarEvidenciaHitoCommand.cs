using MediatR;

namespace Plataforma.Application.Obras.Commands.AgregarEvidenciaHito;

public sealed record AgregarEvidenciaHitoCommand(
    Guid ProyectoObraId,
    Guid HitoId,
    Stream Contenido,
    string NombreArchivo,
    string ContentType
) : IRequest<AgregarEvidenciaHitoResult?>;

public sealed record AgregarEvidenciaHitoResult(Guid HitoId, string FotoEvidenciaUrl);
