using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Tarifas.Commands.Common;

namespace Plataforma.Application.Tarifas.Queries.ObtenerPaquetesTarifaAdmin;

public sealed class ObtenerPaquetesTarifaAdminQueryHandler
    : IRequestHandler<ObtenerPaquetesTarifaAdminQuery, IReadOnlyList<PaqueteTarifaResult>>
{
    private readonly IPaqueteTarifaRepository _repository;

    public ObtenerPaquetesTarifaAdminQueryHandler(IPaqueteTarifaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PaqueteTarifaResult>> Handle(ObtenerPaquetesTarifaAdminQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.ListAsync(cancellationToken);
        return items.Select(p => p.ToResult()).ToList();
    }
}
