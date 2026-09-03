using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Confianza.Commands.Common;
using Plataforma.Domain.Confianza;

namespace Plataforma.Application.Confianza.Commands.CrearContenidoConfianza;

public sealed class CrearContenidoConfianzaCommandHandler : IRequestHandler<CrearContenidoConfianzaCommand, ContenidoConfianzaResult>
{
    private readonly IContenidoConfianzaRepository _repository;

    public CrearContenidoConfianzaCommandHandler(IContenidoConfianzaRepository repository)
    {
        _repository = repository;
    }

    public async Task<ContenidoConfianzaResult> Handle(CrearContenidoConfianzaCommand request, CancellationToken cancellationToken)
    {
        var contenido = ContenidoConfianza.Crear(
            request.Tipo, request.Titulo, request.Descripcion, request.Municipio, request.ServicioRelacionado);

        await _repository.AddAsync(contenido, cancellationToken);

        return contenido.ToResult();
    }
}
