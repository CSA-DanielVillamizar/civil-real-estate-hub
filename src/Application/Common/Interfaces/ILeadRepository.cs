using Plataforma.Domain.Leads;

namespace Plataforma.Application.Common.Interfaces;

public interface ILeadRepository
{
    Task<Lead?> GetByIdAsync(LeadId id, CancellationToken cancellationToken);

    Task AddAsync(Lead lead, CancellationToken cancellationToken);
}
