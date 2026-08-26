using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.ValueObjects;

public sealed class RetiroAmbiental : ValueObject
{
    public TipoFuenteRetiro TipoFuente { get; }
    public decimal DistanciaMinimaMetros { get; }
    public string NormativaAplicable { get; }

    // Reservado para materialización de EF Core (Fase 4).
    private RetiroAmbiental() { }

    private RetiroAmbiental(TipoFuenteRetiro tipoFuente, decimal distanciaMinimaMetros, string normativaAplicable)
    {
        TipoFuente = tipoFuente;
        DistanciaMinimaMetros = distanciaMinimaMetros;
        NormativaAplicable = normativaAplicable;
    }

    public static RetiroAmbiental Crear(TipoFuenteRetiro tipoFuente, decimal distanciaMinimaMetros, string normativaAplicable)
    {
        if (distanciaMinimaMetros <= 0)
            throw new ArgumentException("La distancia mínima debe ser mayor que cero.", nameof(distanciaMinimaMetros));

        if (string.IsNullOrWhiteSpace(normativaAplicable))
            throw new ArgumentException("La normativa aplicable es obligatoria.", nameof(normativaAplicable));

        return new RetiroAmbiental(tipoFuente, distanciaMinimaMetros, normativaAplicable.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return TipoFuente;
        yield return DistanciaMinimaMetros;
        yield return NormativaAplicable;
    }
}
