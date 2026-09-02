using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Obras.Queries.Common;
using Plataforma.Domain.Obras;

namespace Plataforma.Application.Obras.Queries.ObtenerProyectoObraPorId;

public sealed class ObtenerProyectoObraPorIdQueryHandler : IRequestHandler<ObtenerProyectoObraPorIdQuery, ProyectoObraDetalle?>
{
    private readonly IProyectoObraRepository _proyectoObraRepository;

    public ObtenerProyectoObraPorIdQueryHandler(IProyectoObraRepository proyectoObraRepository)
    {
        _proyectoObraRepository = proyectoObraRepository;
    }

    public async Task<ProyectoObraDetalle?> Handle(ObtenerProyectoObraPorIdQuery request, CancellationToken cancellationToken)
    {
        var proyecto = await _proyectoObraRepository.GetByIdAsync(new ProyectoObraId(request.Id), cancellationToken);
        return proyecto?.ToDetalle();
    }
}
