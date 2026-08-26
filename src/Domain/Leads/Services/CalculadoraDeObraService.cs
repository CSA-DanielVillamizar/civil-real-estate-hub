using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.SharedKernel;

namespace Plataforma.Domain.Leads.Services;

// Domain Service puro: sin dependencias externas (sin I/O, sin repositorios).
// Ver Plataforma.Domain.Leads.Services.TarifarioObra para las tarifas usadas
// (placeholder, pendientes de validación comercial).
public sealed class CalculadoraDeObraService
{
    public EstimacionCosto Calcular(DatosCalculoObra datos)
    {
        ArgumentNullException.ThrowIfNull(datos);

        var costoBasePorM2 = TarifarioObra.CostoBasePorM2[datos.TipoAcabado];
        var costoBaseTotal = costoBasePorM2 * datos.AreaConstruccionM2;

        var montoMinimo = Dinero.Crear(costoBaseTotal * TarifarioObra.FactorMinimo);
        var montoMaximo = Dinero.Crear(costoBaseTotal * TarifarioObra.FactorMaximo);

        var desglose = TarifarioObra.DistribucionPorCategoria
            .Select(d => DesgloseItem.Crear(d.Categoria, Dinero.Crear(costoBaseTotal * d.Porcentaje)))
            .ToList();

        return EstimacionCosto.Crear(montoMinimo, montoMaximo, datos, desglose);
    }
}
