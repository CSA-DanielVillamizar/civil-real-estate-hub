using Plataforma.Domain.Confianza;

namespace Plataforma.Application.Common.Interfaces;

public interface IContenidoConfianzaRepository
{
    Task<ContenidoConfianza?> GetByIdAsync(ContenidoConfianzaId id, CancellationToken cancellationToken);

    Task AddAsync(ContenidoConfianza contenido, CancellationToken cancellationToken);

    Task UpdateAsync(ContenidoConfianza contenido, CancellationToken cancellationToken);

    // Listado administrativo: cualquier estado (publicado o no).
    Task<IReadOnlyList<ContenidoConfianza>> ListAsync(CancellationToken cancellationToken);

    // Listado público: solo Publicado=true, más reciente primero.
    Task<IReadOnlyList<ContenidoConfianza>> ListPublicadosAsync(CancellationToken cancellationToken);
}
