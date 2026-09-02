using Plataforma.Domain.Common;
using Plataforma.Domain.Leads.ValueObjects;

namespace Plataforma.Domain.Usuarios;

public sealed class Usuario : AggregateRoot<UsuarioId>
{
    public string Nombre { get; private set; }
    public Email Email { get; private set; }

    // Hash calculado por Infrastructure (PasswordHasher<Usuario>, PBKDF2) —
    // el dominio nunca ve ni valida la contraseña en texto plano.
    public string PasswordHash { get; private set; }

    public RolUsuario Rol { get; private set; }
    public bool Activo { get; private set; }
    public DateTimeOffset CreadoEn { get; private set; }

    // Reservado para materialización de EF Core.
    private Usuario() { }

    private Usuario(UsuarioId id, string nombre, Email email, string passwordHash, RolUsuario rol) : base(id)
    {
        Nombre = nombre;
        Email = email;
        PasswordHash = passwordHash;
        Rol = rol;
        Activo = true;
        CreadoEn = DateTimeOffset.UtcNow;
    }

    public static Usuario Crear(string nombre, Email email, string passwordHash, RolUsuario rol)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

        ArgumentNullException.ThrowIfNull(email);

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El hash de la contraseña es obligatorio.", nameof(passwordHash));

        return new Usuario(UsuarioId.Nueva(), nombre.Trim(), email, passwordHash, rol);
    }

    // Usado solo por el flujo de login para actualizar el hash cuando
    // PasswordHasher<T> señala que el hash existente usa un algoritmo/costo
    // desactualizado (rehash-on-verify, patrón estándar de Identity).
    public void ActualizarPasswordHash(string nuevoPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(nuevoPasswordHash))
            throw new ArgumentException("El hash de la contraseña es obligatorio.", nameof(nuevoPasswordHash));

        PasswordHash = nuevoPasswordHash;
    }
}
