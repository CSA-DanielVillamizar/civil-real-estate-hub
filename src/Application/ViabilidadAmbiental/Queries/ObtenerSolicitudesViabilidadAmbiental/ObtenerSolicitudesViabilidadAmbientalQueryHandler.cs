using MediatR;
using Plataforma.Application.Common.Interfaces;

namespace Plataforma.Application.ViabilidadAmbiental.Queries.ObtenerSolicitudesViabilidadAmbiental;

public sealed class ObtenerSolicitudesViabilidadAmbientalQueryHandler
    : IRequestHandler<ObtenerSolicitudesViabilidadAmbientalQuery, IReadOnlyList<SolicitudViabilidadAmbientalListItem>>
{
    private readonly ISolicitudViabilidadAmbientalRepository _repository;

    public ObtenerSolicitudesViabilidadAmbientalQueryHandler(ISolicitudViabilidadAmbientalRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SolicitudViabilidadAmbientalListItem>> Handle(
        ObtenerSolicitudesViabilidadAmbientalQuery request, CancellationToken cancellationToken)
    {
        var solicitudes = await _repository.ListAsync(request.Estado, cancellationToken);

        return solicitudes
            .Select(s => new SolicitudViabilidadAmbientalListItem(
                s.Id.Value,
                s.Solicitante.Nombre,
                s.Solicitante.Email.Valor,
                s.Solicitante.Telefono.ToString(),
                s.PropiedadId?.Value,
                s.UbicacionLote?.Municipio,
                s.UbicacionLote?.Departamento,
                s.Monto.Monto,
                s.Monto.Moneda,
                s.Estado.ToString(),
                s.SolicitadaEn,
                s.PagoConfirmadoEn))
            .ToList();
    }
}
