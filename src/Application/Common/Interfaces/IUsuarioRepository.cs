using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Usuarios;

namespace Plataforma.Application.Common.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByEmailAsync(Email email, CancellationToken cancellationToken);

    Task<Usuario?> GetByIdAsync(UsuarioId id, CancellationToken cancellationToken);

    Task AddAsync(Usuario usuario, CancellationToken cancellationToken);

    Task UpdateAsync(Usuario usuario, CancellationToken cancellationToken);

    // Usado solo por el bootstrap del primer administrador al iniciar la
    // aplicación (ver Infrastructure/Auth/AdminBootstrapper), para decidir si
    // ya existe al menos un usuario y evitar crear duplicados en cada reinicio.
    Task<bool> ExisteAlgunoAsync(CancellationToken cancellationToken);

    // Panel administrativo — sin paginación, mismo criterio que el resto de
    // listados admin del MVP (volumen esperado bajo: un puñado de Admin/
    // AsesorComercial, no miles).
    Task<IReadOnlyList<Usuario>> ListAsync(CancellationToken cancellationToken);
}
