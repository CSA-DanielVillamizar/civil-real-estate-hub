using Microsoft.EntityFrameworkCore;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads;

namespace Plataforma.Infrastructure.Persistence.Repositories;

public sealed class LeadRepository : ILeadRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LeadRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Lead?> GetByIdAsync(LeadId id, CancellationToken cancellationToken) =>
        await _dbContext.Leads.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    // Ver nota en PropertyRepository.AddAsync sobre la ausencia de un
    // IUnitOfWork independiente en los contratos aprobados en Fase 3.
    public async Task AddAsync(Lead lead, CancellationToken cancellationToken)
    {
        await _dbContext.Leads.AddAsync(lead, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Lead lead, CancellationToken cancellationToken)
    {
        // Update() es un no-op seguro si la entidad ya está rastreada (el
        // caso normal: viene de un GetByIdAsync en el mismo scope) — protege
        // igual si en el futuro se llama con una entidad "desconectada".
        _dbContext.Leads.Update(lead);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
