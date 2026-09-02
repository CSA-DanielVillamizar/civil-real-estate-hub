using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Obras.Queries.Common;

namespace Plataforma.Application.Obras.Queries.ObtenerProyectoObraPorToken;

public sealed class ObtenerProyectoObraPorTokenQueryHandler : IRequestHandler<ObtenerProyectoObraPorTokenQuery, ProyectoObraDetalle?>
{
    private readonly IProyectoObraRepository _proyectoObraRepository;

    public ObtenerProyectoObraPorTokenQueryHandler(IProyectoObraRepository proyectoObraRepository)
    {
        _proyectoObraRepository = proyectoObraRepository;
    }

    public async Task<ProyectoObraDetalle?> Handle(ObtenerProyectoObraPorTokenQuery request, CancellationToken cancellationToken)
    {
        var proyecto = await _proyectoObraRepository.GetByTokenAsync(request.Token, cancellationToken);
        return proyecto?.ToDetalle();
    }
}
