using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Tarifas.Commands.Common;
using Plataforma.Domain.Tarifas;

namespace Plataforma.Application.Tarifas.Commands.CrearPaqueteTarifa;

public sealed class CrearPaqueteTarifaCommandHandler : IRequestHandler<CrearPaqueteTarifaCommand, PaqueteTarifaResult>
{
    private readonly IPaqueteTarifaRepository _repository;

    public CrearPaqueteTarifaCommandHandler(IPaqueteTarifaRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaqueteTarifaResult> Handle(CrearPaqueteTarifaCommand request, CancellationToken cancellationToken)
    {
        var paquete = PaqueteTarifa.Crear(
            request.ServicioRelacionado, request.Titulo, request.Descripcion,
            request.PrecioDesde, request.PrecioHasta, request.UnidadPrecio, request.Moneda);

        await _repository.AddAsync(paquete, cancellationToken);

        return paquete.ToResult();
    }
}
