using Plataforma.Domain.Leads;

namespace Plataforma.Application.Common.Interfaces;

public interface ILeadRepository
{
    Task<Lead?> GetByIdAsync(LeadId id, CancellationToken cancellationToken);

    Task AddAsync(Lead lead, CancellationToken cancellationToken);

    // Persiste cambios sobre un Lead ya existente (ej. marcar la notificación
    // comercial como enviada) — ver la misma nota de AddAsync sobre por qué
    // esto confirma de inmediato en vez de exponer un IUnitOfWork separado.
    Task UpdateAsync(Lead lead, CancellationToken cancellationToken);

    // Panel administrativo (CRM mínimo) — sin paginación, mismo criterio que
    // ISolicitudViabilidadAmbientalRepository.ListAsync: volumen esperado
    // bajo para el MVP. Más reciente primero.
    Task<IReadOnlyList<Lead>> ListAsync(EstadoLead? estado, CancellationToken cancellationToken);
}
