namespace Plataforma.Domain.Obras;

public readonly record struct ProyectoObraId(Guid Value)
{
    public static ProyectoObraId Nueva() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
