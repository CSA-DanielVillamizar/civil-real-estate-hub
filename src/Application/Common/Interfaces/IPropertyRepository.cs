using Plataforma.Domain.Propiedades;

namespace Plataforma.Application.Common.Interfaces;

public interface IPropertyRepository
{
    // NOTA PARA INFRAESTRUCTURA (Fase 4): "SoloViablesConstructivamente" expresa un
    // criterio de negocio calculado por Propiedad.EvaluarViabilidadConstructiva()
    // (Domain), no un dato almacenado. La implementación concreta decide cómo
    // resolverlo (columna calculada persistida y sincronizada, traducción del
    // mismo criterio a SQL, o post-filtrado) — debe mantenerse equivalente a la
    // regla del dominio para no divergir.
    Task<(IReadOnlyList<Propiedad> Items, int TotalCount)> SearchAsync(
        PropertyFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<Propiedad?> GetByIdAsync(PropiedadId id, CancellationToken cancellationToken);

    Task AddAsync(Propiedad propiedad, CancellationToken cancellationToken);

    Task UpdateAsync(Propiedad propiedad, CancellationToken cancellationToken);
}
