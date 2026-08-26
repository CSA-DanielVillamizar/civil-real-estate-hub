using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.ValueObjects;

public sealed class Area : ValueObject
{
    public decimal Valor { get; }
    public UnidadMedidaArea UnidadMedida { get; }

    // Reservado para materialización de EF Core (Fase 4).
    private Area() { }

    private Area(decimal valor, UnidadMedidaArea unidadMedida)
    {
        Valor = valor;
        UnidadMedida = unidadMedida;
    }

    public static Area Crear(decimal valor, UnidadMedidaArea unidadMedida = UnidadMedidaArea.M2)
    {
        if (valor <= 0)
            throw new ArgumentException("El área debe ser mayor que cero.", nameof(valor));

        return new Area(valor, unidadMedida);
    }

    public decimal EnMetrosCuadrados() => UnidadMedida == UnidadMedidaArea.Hectarea ? Valor * 10_000m : Valor;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
        yield return UnidadMedida;
    }

    public override string ToString() => $"{Valor} {UnidadMedida}";
}
