using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Obras;

namespace Plataforma.Application.Obras.Queries.ObtenerProyectosObra;

public sealed class ObtenerProyectosObraQueryHandler : IRequestHandler<ObtenerProyectosObraQuery, IReadOnlyList<ProyectoObraListItem>>
{
    private readonly IProyectoObraRepository _proyectoObraRepository;

    public ObtenerProyectosObraQueryHandler(IProyectoObraRepository proyectoObraRepository)
    {
        _proyectoObraRepository = proyectoObraRepository;
    }

    public async Task<IReadOnlyList<ProyectoObraListItem>> Handle(ObtenerProyectosObraQuery request, CancellationToken cancellationToken)
    {
        var proyectos = await _proyectoObraRepository.ListAsync(cancellationToken);

        return proyectos
            .OrderByDescending(p => p.CreadoEn)
            .Select(p => new ProyectoObraListItem(
                p.Id.Value,
                p.NombreCliente,
                p.NombreProyecto,
                p.Estado,
                p.CreadoEn,
                p.Hitos.Count,
                p.Hitos.Count(h => h.Estado == EstadoHito.Completado),
                p.TokenAcceso))
            .ToList();
    }
}
