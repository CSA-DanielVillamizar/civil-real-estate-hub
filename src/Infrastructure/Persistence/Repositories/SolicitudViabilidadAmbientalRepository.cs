using Microsoft.EntityFrameworkCore;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.ViabilidadAmbiental;

namespace Plataforma.Infrastructure.Persistence.Repositories;

public sealed class SolicitudViabilidadAmbientalRepository : ISolicitudViabilidadAmbientalRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SolicitudViabilidadAmbientalRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SolicitudViabilidadAmbiental?> GetByIdAsync(SolicitudViabilidadAmbientalId id, CancellationToken cancellationToken) =>
        await _dbContext.SolicitudesViabilidadAmbiental.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task AddAsync(SolicitudViabilidadAmbiental solicitud, CancellationToken cancellationToken)
    {
        await _dbContext.SolicitudesViabilidadAmbiental.AddAsync(solicitud, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SolicitudViabilidadAmbiental solicitud, CancellationToken cancellationToken)
    {
        _dbContext.SolicitudesViabilidadAmbiental.Update(solicitud);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SolicitudViabilidadAmbiental>> ListAsync(
        EstadoSolicitudViabilidad? estado, CancellationToken cancellationToken)
    {
        var query = _dbContext.SolicitudesViabilidadAmbiental.AsNoTracking().AsQueryable();

        if (estado is not null)
            query = query.Where(s => s.Estado == estado);

        return await query.OrderByDescending(s => s.SolicitadaEn).ToListAsync(cancellationToken);
    }
}
