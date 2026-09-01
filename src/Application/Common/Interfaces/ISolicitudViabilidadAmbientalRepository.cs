using Plataforma.Domain.ViabilidadAmbiental;

namespace Plataforma.Application.Common.Interfaces;

public interface ISolicitudViabilidadAmbientalRepository
{
    Task<SolicitudViabilidadAmbiental?> GetByIdAsync(SolicitudViabilidadAmbientalId id, CancellationToken cancellationToken);

    Task AddAsync(SolicitudViabilidadAmbiental solicitud, CancellationToken cancellationToken);

    Task UpdateAsync(SolicitudViabilidadAmbiental solicitud, CancellationToken cancellationToken);

    // Sin paginación — volumen esperado bajo para el panel administrativo del
    // MVP (ver GetPropertiesQuery/PropertyRepository.SearchAsync si en algún
    // momento hace falta paginar). Más reciente primero.
    Task<IReadOnlyList<SolicitudViabilidadAmbiental>> ListAsync(EstadoSolicitudViabilidad? estado, CancellationToken cancellationToken);
}
