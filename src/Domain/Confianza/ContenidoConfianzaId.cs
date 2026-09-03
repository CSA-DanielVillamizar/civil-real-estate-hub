namespace Plataforma.Domain.Confianza;

public readonly record struct ContenidoConfianzaId(Guid Value)
{
    public static ContenidoConfianzaId Nueva() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
