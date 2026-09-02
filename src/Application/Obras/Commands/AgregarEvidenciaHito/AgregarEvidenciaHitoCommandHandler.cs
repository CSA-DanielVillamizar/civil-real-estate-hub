using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Obras;

namespace Plataforma.Application.Obras.Commands.AgregarEvidenciaHito;

public sealed class AgregarEvidenciaHitoCommandHandler : IRequestHandler<AgregarEvidenciaHitoCommand, AgregarEvidenciaHitoResult?>
{
    private readonly IProyectoObraRepository _proyectoObraRepository;
    private readonly IObraEvidenciaStorage _evidenciaStorage;

    public AgregarEvidenciaHitoCommandHandler(IProyectoObraRepository proyectoObraRepository, IObraEvidenciaStorage evidenciaStorage)
    {
        _proyectoObraRepository = proyectoObraRepository;
        _evidenciaStorage = evidenciaStorage;
    }

    public async Task<AgregarEvidenciaHitoResult?> Handle(AgregarEvidenciaHitoCommand request, CancellationToken cancellationToken)
    {
        var proyecto = await _proyectoObraRepository.GetByIdAsync(new ProyectoObraId(request.ProyectoObraId), cancellationToken);
        if (proyecto is null)
            return null;

        // Sube primero: si Blob Storage falla, no queda una URL inexistente
        // grabada en el hito (mismo criterio que AgregarMultimediaAPropiedad).
        var url = await _evidenciaStorage.SubirAsync(request.Contenido, request.NombreArchivo, request.ContentType, cancellationToken);

        proyecto.AgregarEvidenciaAHito(request.HitoId, url);
        await _proyectoObraRepository.UpdateAsync(proyecto, cancellationToken);

        return new AgregarEvidenciaHitoResult(request.HitoId, url);
    }
}
