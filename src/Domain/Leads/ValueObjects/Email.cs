using System.Text.RegularExpressions;
using Plataforma.Domain.Common;

namespace Plataforma.Domain.Leads.ValueObjects;

public sealed partial class Email : ValueObject
{
    public string Valor { get; }

    // Reservado para materialización de EF Core (Fase 4).
    private Email() { }

    private Email(string valor) => Valor = valor;

    public static Email Crear(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("El email es obligatorio.", nameof(valor));

        var normalizado = valor.Trim();

        if (!FormatoEmailRegex().IsMatch(normalizado))
            throw new ArgumentException("El formato del email no es válido.", nameof(valor));

        return new Email(normalizado);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor.ToLowerInvariant();
    }

    public override string ToString() => Valor;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex FormatoEmailRegex();
}
