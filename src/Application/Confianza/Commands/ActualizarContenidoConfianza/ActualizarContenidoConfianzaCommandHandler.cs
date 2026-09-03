using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Confianza.Commands.Common;
using Plataforma.Domain.Confianza;

namespace Plataforma.Application.Confianza.Commands.ActualizarContenidoConfianza;

public sealed class ActualizarContenidoConfianzaCommandHandler
    : IRequestHandler<ActualizarContenidoConfianzaCommand, ContenidoConfianzaResult?>
{
    private readonly IContenidoConfianzaRepository _repository;

    public ActualizarContenidoConfianzaCommandHandler(IContenidoConfianzaRepository repository)
    {
        _repository = repository;
    }

    public async Task<ContenidoConfianzaResult?> Handle(ActualizarContenidoConfianzaCommand request, CancellationToken cancellationToken)
    {
        var contenido = await _repository.GetByIdAsync(new ContenidoConfianzaId(request.ContenidoId), cancellationToken);
        if (contenido is null)
            return null;

        contenido.Actualizar(request.Titulo, request.Descripcion, request.Municipio, request.ServicioRelacionado);

        await _repository.UpdateAsync(contenido, cancellationToken);

        return contenido.ToResult();
    }
}
