using System.Text.RegularExpressions;
using Plataforma.Domain.Common;

namespace Plataforma.Domain.Leads.ValueObjects;

public sealed partial class Telefono : ValueObject
{
    public const string IndicativoPorDefecto = "+57";

    public string Numero { get; }
    public string Indicativo { get; }

    // Reservado para materialización de EF Core (Fase 4).
    private Telefono() { }

    private Telefono(string numero, string indicativo)
    {
        Numero = numero;
        Indicativo = indicativo;
    }

    public static Telefono Crear(string numero, string? indicativo = null)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("El teléfono es obligatorio.", nameof(numero));

        if (!FormatoNumeroRegex().IsMatch(numero))
            throw new ArgumentException("El teléfono debe contener solo dígitos (7 a 15).", nameof(numero));

        var indicativoNormalizado = string.IsNullOrWhiteSpace(indicativo) ? IndicativoPorDefecto : indicativo.Trim();

        if (!FormatoIndicativoRegex().IsMatch(indicativoNormalizado))
            throw new ArgumentException("El indicativo debe tener el formato +57.", nameof(indicativo));

        return new Telefono(numero, indicativoNormalizado);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Numero;
        yield return Indicativo;
    }

    public override string ToString() => $"{Indicativo}{Numero}";

    [GeneratedRegex(@"^[0-9]{7,15}$")]
    private static partial Regex FormatoNumeroRegex();

    [GeneratedRegex(@"^\+[0-9]{1,4}$")]
    private static partial Regex FormatoIndicativoRegex();
}
