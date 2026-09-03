using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Tarifas.Commands.Common;
using Plataforma.Domain.Tarifas;

namespace Plataforma.Application.Tarifas.Commands.DespublicarPaqueteTarifa;

public sealed class DespublicarPaqueteTarifaCommandHandler : IRequestHandler<DespublicarPaqueteTarifaCommand, PaqueteTarifaResult?>
{
    private readonly IPaqueteTarifaRepository _repository;

    public DespublicarPaqueteTarifaCommandHandler(IPaqueteTarifaRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaqueteTarifaResult?> Handle(DespublicarPaqueteTarifaCommand request, CancellationToken cancellationToken)
    {
        var paquete = await _repository.GetByIdAsync(new PaqueteTarifaId(request.PaqueteId), cancellationToken);
        if (paquete is null)
            return null;

        paquete.Despublicar();
        await _repository.UpdateAsync(paquete, cancellationToken);

        return paquete.ToResult();
    }
}
