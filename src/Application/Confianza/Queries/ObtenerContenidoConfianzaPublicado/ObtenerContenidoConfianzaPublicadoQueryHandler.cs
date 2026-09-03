using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Confianza.Commands.Common;

namespace Plataforma.Application.Confianza.Queries.ObtenerContenidoConfianzaPublicado;

public sealed class ObtenerContenidoConfianzaPublicadoQueryHandler
    : IRequestHandler<ObtenerContenidoConfianzaPublicadoQuery, IReadOnlyList<ContenidoConfianzaResult>>
{
    private readonly IContenidoConfianzaRepository _repository;

    public ObtenerContenidoConfianzaPublicadoQueryHandler(IContenidoConfianzaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ContenidoConfianzaResult>> Handle(
        ObtenerContenidoConfianzaPublicadoQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.ListPublicadosAsync(cancellationToken);
        return items.Select(c => c.ToResult()).ToList();
    }
}
