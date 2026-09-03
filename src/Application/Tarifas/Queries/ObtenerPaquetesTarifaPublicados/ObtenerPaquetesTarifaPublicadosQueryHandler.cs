using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Tarifas.Commands.Common;

namespace Plataforma.Application.Tarifas.Queries.ObtenerPaquetesTarifaPublicados;

public sealed class ObtenerPaquetesTarifaPublicadosQueryHandler
    : IRequestHandler<ObtenerPaquetesTarifaPublicadosQuery, IReadOnlyList<PaqueteTarifaResult>>
{
    private readonly IPaqueteTarifaRepository _repository;

    public ObtenerPaquetesTarifaPublicadosQueryHandler(IPaqueteTarifaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PaqueteTarifaResult>> Handle(ObtenerPaquetesTarifaPublicadosQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.ListPublicadosAsync(cancellationToken);
        return items.Select(p => p.ToResult()).ToList();
    }
}
