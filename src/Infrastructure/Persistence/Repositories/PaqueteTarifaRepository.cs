using Microsoft.EntityFrameworkCore;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Tarifas;

namespace Plataforma.Infrastructure.Persistence.Repositories;

public sealed class PaqueteTarifaRepository : IPaqueteTarifaRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PaqueteTarifaRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaqueteTarifa?> GetByIdAsync(PaqueteTarifaId id, CancellationToken cancellationToken) =>
        await _dbContext.PaquetesTarifa.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task AddAsync(PaqueteTarifa paquete, CancellationToken cancellationToken)
    {
        await _dbContext.PaquetesTarifa.AddAsync(paquete, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PaqueteTarifa paquete, CancellationToken cancellationToken)
    {
        _dbContext.PaquetesTarifa.Update(paquete);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PaqueteTarifa>> ListAsync(CancellationToken cancellationToken) =>
        await _dbContext.PaquetesTarifa.AsNoTracking()
            .OrderByDescending(p => p.CreadoEn)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PaqueteTarifa>> ListPublicadosAsync(CancellationToken cancellationToken) =>
        await _dbContext.PaquetesTarifa.AsNoTracking()
            .Where(p => p.Publicado)
            .OrderByDescending(p => p.CreadoEn)
            .ToListAsync(cancellationToken);
}
