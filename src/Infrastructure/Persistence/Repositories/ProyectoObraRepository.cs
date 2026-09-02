using Microsoft.EntityFrameworkCore;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Obras;

namespace Plataforma.Infrastructure.Persistence.Repositories;

public sealed class ProyectoObraRepository : IProyectoObraRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProyectoObraRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProyectoObra?> GetByIdAsync(ProyectoObraId id, CancellationToken cancellationToken) =>
        await _dbContext.ProyectosObra.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<ProyectoObra?> GetByTokenAsync(string token, CancellationToken cancellationToken) =>
        await _dbContext.ProyectosObra.FirstOrDefaultAsync(p => p.TokenAcceso == token, cancellationToken);

    public async Task AddAsync(ProyectoObra proyecto, CancellationToken cancellationToken)
    {
        await _dbContext.ProyectosObra.AddAsync(proyecto, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ProyectoObra proyecto, CancellationToken cancellationToken)
    {
        _dbContext.ProyectosObra.Update(proyecto);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProyectoObra>> ListAsync(CancellationToken cancellationToken) =>
        await _dbContext.ProyectosObra.AsNoTracking().OrderByDescending(p => p.CreadoEn).ToListAsync(cancellationToken);
}
