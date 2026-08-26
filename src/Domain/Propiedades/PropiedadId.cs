namespace Plataforma.Domain.Propiedades;

public readonly record struct PropiedadId(Guid Value)
{
    public static PropiedadId Nueva() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
