namespace Plataforma.Domain.Tarifas;

public readonly record struct PaqueteTarifaId(Guid Value)
{
    public static PaqueteTarifaId Nueva() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
