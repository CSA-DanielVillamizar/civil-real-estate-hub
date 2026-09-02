using Plataforma.Domain.Obras;

namespace Plataforma.Application.Common.Interfaces;

public interface IProyectoObraRepository
{
    Task<ProyectoObra?> GetByIdAsync(ProyectoObraId id, CancellationToken cancellationToken);

    Task<ProyectoObra?> GetByTokenAsync(string token, CancellationToken cancellationToken);

    Task AddAsync(ProyectoObra proyecto, CancellationToken cancellationToken);

    Task UpdateAsync(ProyectoObra proyecto, CancellationToken cancellationToken);

    // Panel administrativo — sin paginación, mismo criterio que
    // ILeadRepository.ListAsync: volumen esperado bajo para el MVP.
    Task<IReadOnlyList<ProyectoObra>> ListAsync(CancellationToken cancellationToken);
}
