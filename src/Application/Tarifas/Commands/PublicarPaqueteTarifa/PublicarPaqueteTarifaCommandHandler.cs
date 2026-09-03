using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Tarifas.Commands.Common;
using Plataforma.Domain.Tarifas;

namespace Plataforma.Application.Tarifas.Commands.PublicarPaqueteTarifa;

public sealed class PublicarPaqueteTarifaCommandHandler : IRequestHandler<PublicarPaqueteTarifaCommand, PaqueteTarifaResult?>
{
    private readonly IPaqueteTarifaRepository _repository;

    public PublicarPaqueteTarifaCommandHandler(IPaqueteTarifaRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaqueteTarifaResult?> Handle(PublicarPaqueteTarifaCommand request, CancellationToken cancellationToken)
    {
        var paquete = await _repository.GetByIdAsync(new PaqueteTarifaId(request.PaqueteId), cancellationToken);
        if (paquete is null)
            return null;

        paquete.Publicar();
        await _repository.UpdateAsync(paquete, cancellationToken);

        return paquete.ToResult();
    }
}
