using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.ValueObjects;

public sealed class CaracteristicasTopograficas : ValueObject
{
    public decimal PendientePorcentaje { get; }
    public TipoSuelo TipoSuelo { get; }
    public Topografia Topografia { get; }
    public decimal? NivelFreaticoMetros { get; }

    // Reservado para materialización de EF Core (Fase 4).
    private CaracteristicasTopograficas() { }

    private CaracteristicasTopograficas(
        decimal pendientePorcentaje,
        TipoSuelo tipoSuelo,
        Topografia topografia,
        decimal? nivelFreaticoMetros)
    {
        PendientePorcentaje = pendientePorcentaje;
        TipoSuelo = tipoSuelo;
        Topografia = topografia;
        NivelFreaticoMetros = nivelFreaticoMetros;
    }

    public static CaracteristicasTopograficas Crear(
        decimal pendientePorcentaje,
        TipoSuelo tipoSuelo,
        Topografia topografia,
        decimal? nivelFreaticoMetros = null)
    {
        if (pendientePorcentaje < 0)
            throw new ArgumentException("La pendiente no puede ser negativa.", nameof(pendientePorcentaje));

        if (nivelFreaticoMetros is < 0)
            throw new ArgumentException("El nivel freático no puede ser negativo.", nameof(nivelFreaticoMetros));

        return new CaracteristicasTopograficas(pendientePorcentaje, tipoSuelo, topografia, nivelFreaticoMetros);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return PendientePorcentaje;
        yield return TipoSuelo;
        yield return Topografia;
        yield return NivelFreaticoMetros;
    }
}
