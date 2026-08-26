using Plataforma.Contracts.Common;
using DomainDatosCalculoObra = Plataforma.Domain.Leads.ValueObjects.DatosCalculoObra;
using DomainEstimacionCosto = Plataforma.Domain.Leads.ValueObjects.EstimacionCosto;

namespace Plataforma.WebApi.Mapping;

public static class BudgetsMapping
{
    public static DomainDatosCalculoObra ToDomain(this DatosCalculoObraDto dto) =>
        DomainDatosCalculoObra.Crear(dto.AreaConstruccionM2, dto.TipoAcabado.ToDomain(), dto.Municipio, dto.TipoProyecto.ToDomain());

    public static EstimacionCostoDto ToContract(this DomainEstimacionCosto estimacion) =>
        new(
            estimacion.MontoMinimo.Monto,
            estimacion.MontoMaximo.Monto,
            estimacion.MontoMinimo.Moneda,
            estimacion.Desglose
                .Select(item => new DesgloseItemDto(item.Categoria, item.Monto.Monto))
                .ToList());
}
