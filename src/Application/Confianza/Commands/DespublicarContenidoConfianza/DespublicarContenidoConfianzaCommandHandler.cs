using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Confianza.Commands.Common;
using Plataforma.Domain.Confianza;

namespace Plataforma.Application.Confianza.Commands.DespublicarContenidoConfianza;

public sealed class DespublicarContenidoConfianzaCommandHandler
    : IRequestHandler<DespublicarContenidoConfianzaCommand, ContenidoConfianzaResult?>
{
    private readonly IContenidoConfianzaRepository _repository;

    public DespublicarContenidoConfianzaCommandHandler(IContenidoConfianzaRepository repository)
    {
        _repository = repository;
    }

    public async Task<ContenidoConfianzaResult?> Handle(DespublicarContenidoConfianzaCommand request, CancellationToken cancellationToken)
    {
        var contenido = await _repository.GetByIdAsync(new ContenidoConfianzaId(request.ContenidoId), cancellationToken);
        if (contenido is null)
            return null;

        contenido.Despublicar();
        await _repository.UpdateAsync(contenido, cancellationToken);

        return contenido.ToResult();
    }
}
