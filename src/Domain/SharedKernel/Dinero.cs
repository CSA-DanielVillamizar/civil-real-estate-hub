using Plataforma.Domain.Common;

namespace Plataforma.Domain.SharedKernel;

public sealed class Dinero : ValueObject
{
    public const string MonedaPorDefecto = "COP";

    public decimal Monto { get; }
    public string Moneda { get; }

    // Reservado para materialización de EF Core (Fase 4).
    private Dinero() { }

    private Dinero(decimal monto, string moneda)
    {
        Monto = monto;
        Moneda = moneda;
    }

    public static Dinero Crear(decimal monto, string moneda = MonedaPorDefecto)
    {
        if (monto < 0)
            throw new ArgumentException("El monto no puede ser negativo.", nameof(monto));

        if (string.IsNullOrWhiteSpace(moneda))
            throw new ArgumentException("La moneda es obligatoria.", nameof(moneda));

        return new Dinero(monto, moneda.Trim().ToUpperInvariant());
    }

    public static Dinero Cero(string moneda = MonedaPorDefecto) => Crear(0, moneda);

    public Dinero Sumar(Dinero otro)
    {
        AsegurarMismaMoneda(otro);
        return new Dinero(Monto + otro.Monto, Moneda);
    }

    public Dinero MultiplicarPor(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("El factor no puede ser negativo.", nameof(factor));

        return new Dinero(Monto * factor, Moneda);
    }

    private void AsegurarMismaMoneda(Dinero otro)
    {
        if (otro.Moneda != Moneda)
            throw new InvalidOperationException(
                $"No se pueden combinar montos en monedas distintas ({Moneda} vs {otro.Moneda}).");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Monto;
        yield return Moneda;
    }

    public override string ToString() => $"{Monto:N2} {Moneda}";
}
