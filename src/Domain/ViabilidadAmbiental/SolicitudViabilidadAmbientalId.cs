namespace Plataforma.Domain.ViabilidadAmbiental;

public readonly record struct SolicitudViabilidadAmbientalId(Guid Value)
{
    public static SolicitudViabilidadAmbientalId Nueva() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
