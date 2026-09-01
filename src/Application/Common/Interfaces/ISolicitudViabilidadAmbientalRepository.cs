using Plataforma.Domain.ViabilidadAmbiental;

namespace Plataforma.Application.Common.Interfaces;

public interface ISolicitudViabilidadAmbientalRepository
{
    Task<SolicitudViabilidadAmbiental?> GetByIdAsync(SolicitudViabilidadAmbientalId id, CancellationToken cancellationToken);

    Task AddAsync(SolicitudViabilidadAmbiental solicitud, CancellationToken cancellationToken);

    Task UpdateAsync(SolicitudViabilidadAmbiental solicitud, CancellationToken cancellationToken);
}
