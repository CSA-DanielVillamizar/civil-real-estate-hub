using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Obras;

namespace Plataforma.Application.Obras.Commands.CambiarEstadoProyecto;

public sealed class CambiarEstadoProyectoCommandHandler : IRequestHandler<CambiarEstadoProyectoCommand, CambiarEstadoProyectoResult?>
{
    private readonly IProyectoObraRepository _proyectoObraRepository;

    public CambiarEstadoProyectoCommandHandler(IProyectoObraRepository proyectoObraRepository)
    {
        _proyectoObraRepository = proyectoObraRepository;
    }

    public async Task<CambiarEstadoProyectoResult?> Handle(CambiarEstadoProyectoCommand request, CancellationToken cancellationToken)
    {
        var proyecto = await _proyectoObraRepository.GetByIdAsync(new ProyectoObraId(request.ProyectoObraId), cancellationToken);
        if (proyecto is null)
            return null;

        proyecto.CambiarEstado(request.NuevoEstado);
        await _proyectoObraRepository.UpdateAsync(proyecto, cancellationToken);

        return new CambiarEstadoProyectoResult(proyecto.Id.Value, proyecto.Estado.ToString());
    }
}
