using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Confianza.Commands.Common;

namespace Plataforma.Application.Confianza.Queries.ObtenerContenidoConfianzaAdmin;

public sealed class ObtenerContenidoConfianzaAdminQueryHandler
    : IRequestHandler<ObtenerContenidoConfianzaAdminQuery, IReadOnlyList<ContenidoConfianzaResult>>
{
    private readonly IContenidoConfianzaRepository _repository;

    public ObtenerContenidoConfianzaAdminQueryHandler(IContenidoConfianzaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ContenidoConfianzaResult>> Handle(
        ObtenerContenidoConfianzaAdminQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.ListAsync(cancellationToken);
        return items.Select(c => c.ToResult()).ToList();
    }
}
