namespace Plataforma.Domain.Usuarios;

public readonly record struct UsuarioId(Guid Value)
{
    public static UsuarioId Nueva() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
