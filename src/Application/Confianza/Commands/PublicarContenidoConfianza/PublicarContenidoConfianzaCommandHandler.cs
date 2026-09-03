using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Confianza.Commands.Common;
using Plataforma.Domain.Confianza;

namespace Plataforma.Application.Confianza.Commands.PublicarContenidoConfianza;

public sealed class PublicarContenidoConfianzaCommandHandler
    : IRequestHandler<PublicarContenidoConfianzaCommand, ContenidoConfianzaResult?>
{
    private readonly IContenidoConfianzaRepository _repository;

    public PublicarContenidoConfianzaCommandHandler(IContenidoConfianzaRepository repository)
    {
        _repository = repository;
    }

    public async Task<ContenidoConfianzaResult?> Handle(PublicarContenidoConfianzaCommand request, CancellationToken cancellationToken)
    {
        var contenido = await _repository.GetByIdAsync(new ContenidoConfianzaId(request.ContenidoId), cancellationToken);
        if (contenido is null)
            return null;

        contenido.Publicar();
        await _repository.UpdateAsync(contenido, cancellationToken);

        return contenido.ToResult();
    }
}
