using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Usuarios;

namespace Plataforma.Application.Common.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByEmailAsync(Email email, CancellationToken cancellationToken);

    Task AddAsync(Usuario usuario, CancellationToken cancellationToken);

    Task UpdateAsync(Usuario usuario, CancellationToken cancellationToken);

    // Usado solo por el bootstrap del primer administrador al iniciar la
    // aplicación (ver Infrastructure/Auth/AdminBootstrapper), para decidir si
    // ya existe al menos un usuario y evitar crear duplicados en cada reinicio.
    Task<bool> ExisteAlgunoAsync(CancellationToken cancellationToken);
}
