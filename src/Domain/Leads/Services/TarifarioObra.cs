namespace Plataforma.Domain.Leads.Services;

// ============================================================================
// ASUNCIÓN — TARIFAS PLACEHOLDER, NO VALIDADAS COMERCIALMENTE.
// Estos valores (costo por m² y distribución porcentual por categoría) son
// ilustrativos, para que la calculadora sea funcional y testeable end-to-end.
// NO deben usarse para cotizar obra real sin que el negocio confirme un
// tarifario oficial (por tipo de acabado y, ver nota abajo, por municipio).
//
// La variable "Municipio" de DatosCalculoObra se captura y se conserva en la
// estimación, pero AÚN NO afecta el cálculo — no hay una fuente de datos de
// variación geográfica de costos confirmada. Ajustar cuando se defina.
// ============================================================================
public static class TarifarioObra
{
    public static readonly IReadOnlyDictionary<TipoAcabado, decimal> CostoBasePorM2 = new Dictionary<TipoAcabado, decimal>
    {
        [TipoAcabado.Basico] = 1_800_000m,
        [TipoAcabado.Medio] = 2_600_000m,
        [TipoAcabado.Alto] = 3_800_000m,
    };

    public const decimal FactorMinimo = 0.90m;
    public const decimal FactorMaximo = 1.15m;

    public static readonly IReadOnlyList<(string Categoria, decimal Porcentaje)> DistribucionPorCategoria = new List<(string, decimal)>
    {
        ("ManoDeObra", 0.35m),
        ("Materiales", 0.45m),
        ("Equipos", 0.10m),
        ("AdministracionYUtilidad", 0.10m),
    };
}
