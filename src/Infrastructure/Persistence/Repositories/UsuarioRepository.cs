using Microsoft.EntityFrameworkCore;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Usuarios;

namespace Plataforma.Infrastructure.Persistence.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UsuarioRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Usuario?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        // Comparación sobre la propiedad escalar de la columna owned, no
        // sobre el value object completo: el operador == de ValueObject es
        // un método estático que EF Core no puede traducir a SQL.
        var emailNormalizado = email.Valor.ToLowerInvariant();
        return await _dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.Email.Valor.ToLower() == emailNormalizado, cancellationToken);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken cancellationToken)
    {
        await _dbContext.Usuarios.AddAsync(usuario, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Usuario usuario, CancellationToken cancellationToken)
    {
        _dbContext.Usuarios.Update(usuario);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExisteAlgunoAsync(CancellationToken cancellationToken) =>
        await _dbContext.Usuarios.AsNoTracking().AnyAsync(cancellationToken);
}
