using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Obras;

namespace Plataforma.Application.Obras.Commands.CambiarEstadoHito;

public sealed class CambiarEstadoHitoCommandHandler : IRequestHandler<CambiarEstadoHitoCommand, CambiarEstadoHitoResult?>
{
    private readonly IProyectoObraRepository _proyectoObraRepository;

    public CambiarEstadoHitoCommandHandler(IProyectoObraRepository proyectoObraRepository)
    {
        _proyectoObraRepository = proyectoObraRepository;
    }

    public async Task<CambiarEstadoHitoResult?> Handle(CambiarEstadoHitoCommand request, CancellationToken cancellationToken)
    {
        var proyecto = await _proyectoObraRepository.GetByIdAsync(new ProyectoObraId(request.ProyectoObraId), cancellationToken);
        if (proyecto is null)
            return null;

        // HitoNoEncontradoException (404 lógico dentro del agregado) se
        // deja propagar — el ApplicationExceptionHandler global la traduce
        // a 400, igual que cualquier otra DomainException.
        proyecto.CambiarEstadoHito(request.HitoId, request.NuevoEstado);
        await _proyectoObraRepository.UpdateAsync(proyecto, cancellationToken);

        var hito = proyecto.Hitos.First(h => h.Id == request.HitoId);
        return new CambiarEstadoHitoResult(hito.Id, hito.Estado.ToString());
    }
}
