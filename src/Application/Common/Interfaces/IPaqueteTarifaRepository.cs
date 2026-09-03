using Plataforma.Domain.Tarifas;

namespace Plataforma.Application.Common.Interfaces;

public interface IPaqueteTarifaRepository
{
    Task<PaqueteTarifa?> GetByIdAsync(PaqueteTarifaId id, CancellationToken cancellationToken);

    Task AddAsync(PaqueteTarifa paquete, CancellationToken cancellationToken);

    Task UpdateAsync(PaqueteTarifa paquete, CancellationToken cancellationToken);

    // Listado administrativo: cualquier estado (publicado o no).
    Task<IReadOnlyList<PaqueteTarifa>> ListAsync(CancellationToken cancellationToken);

    // Listado público: solo Publicado=true.
    Task<IReadOnlyList<PaqueteTarifa>> ListPublicadosAsync(CancellationToken cancellationToken);
}
