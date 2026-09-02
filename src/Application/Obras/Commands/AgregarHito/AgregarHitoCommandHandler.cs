using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Obras;

namespace Plataforma.Application.Obras.Commands.AgregarHito;

public sealed class AgregarHitoCommandHandler : IRequestHandler<AgregarHitoCommand, AgregarHitoResult?>
{
    private readonly IProyectoObraRepository _proyectoObraRepository;

    public AgregarHitoCommandHandler(IProyectoObraRepository proyectoObraRepository)
    {
        _proyectoObraRepository = proyectoObraRepository;
    }

    public async Task<AgregarHitoResult?> Handle(AgregarHitoCommand request, CancellationToken cancellationToken)
    {
        var proyecto = await _proyectoObraRepository.GetByIdAsync(new ProyectoObraId(request.ProyectoObraId), cancellationToken);
        if (proyecto is null)
            return null;

        var hito = proyecto.AgregarHito(request.Nombre, request.Descripcion, request.FechaEstimada);
        await _proyectoObraRepository.UpdateAsync(proyecto, cancellationToken);

        return new AgregarHitoResult(hito.Id, hito.Nombre, hito.Estado.ToString());
    }
}
