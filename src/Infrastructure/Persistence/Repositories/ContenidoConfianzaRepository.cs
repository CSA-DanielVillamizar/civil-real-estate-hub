using Microsoft.EntityFrameworkCore;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Confianza;

namespace Plataforma.Infrastructure.Persistence.Repositories;

public sealed class ContenidoConfianzaRepository : IContenidoConfianzaRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ContenidoConfianzaRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ContenidoConfianza?> GetByIdAsync(ContenidoConfianzaId id, CancellationToken cancellationToken) =>
        await _dbContext.ContenidosConfianza.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task AddAsync(ContenidoConfianza contenido, CancellationToken cancellationToken)
    {
        await _dbContext.ContenidosConfianza.AddAsync(contenido, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ContenidoConfianza contenido, CancellationToken cancellationToken)
    {
        _dbContext.ContenidosConfianza.Update(contenido);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ContenidoConfianza>> ListAsync(CancellationToken cancellationToken) =>
        await _dbContext.ContenidosConfianza.AsNoTracking()
            .OrderByDescending(c => c.CreadoEn)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ContenidoConfianza>> ListPublicadosAsync(CancellationToken cancellationToken) =>
        await _dbContext.ContenidosConfianza.AsNoTracking()
            .Where(c => c.Publicado)
            .OrderByDescending(c => c.CreadoEn)
            .ToListAsync(cancellationToken);
}
