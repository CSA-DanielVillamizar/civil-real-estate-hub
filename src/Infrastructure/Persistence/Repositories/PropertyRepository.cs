using Microsoft.EntityFrameworkCore;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.Propiedades.Reglas;

namespace Plataforma.Infrastructure.Persistence.Repositories;

public sealed class PropertyRepository : IPropertyRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PropertyRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<Propiedad> Items, int TotalCount)> SearchAsync(
        PropertyFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Propiedades.AsNoTracking().AsQueryable();

        if (filter.TipoInmueble is not null)
            query = query.Where(p => p.TipoInmueble == filter.TipoInmueble);

        if (!string.IsNullOrWhiteSpace(filter.Municipio))
            query = query.Where(p => p.Ubicacion.Municipio == filter.Municipio);

        if (filter.PrecioMin is not null)
            query = query.Where(p => p.Precio.Monto >= filter.PrecioMin);

        if (filter.PrecioMax is not null)
            query = query.Where(p => p.Precio.Monto <= filter.PrecioMax);

        if (filter.AreaMin is not null)
            query = query.Where(p => p.AreaTerreno.Valor >= filter.AreaMin);

        if (filter.AreaMax is not null)
            query = query.Where(p => p.AreaTerreno.Valor <= filter.AreaMax);

        // Traducción explícita de la regla de dominio (ver
        // Propiedad.CalcularViabilidadConstructiva / ViabilidadConstructivaReglas)
        // a una expresión LINQ traducible por el proveedor de EF Core, para que
        // el filtro se resuelva en PostgreSQL y no en memoria (Prompt 4, ítem 3).
        // Si la regla de negocio del dominio cambia, esta expresión debe
        // actualizarse en conjunto para no divergir.
        if (filter.SoloViablesConstructivamente == true)
        {
            query = query.Where(p =>
                p.CaracteristicasTopograficas.PendientePorcentaje
                    <= ViabilidadConstructivaReglas.PendienteMaximaPermitidaPorcentaje);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Titulo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Propiedad?> GetByIdAsync(PropiedadId id, CancellationToken cancellationToken) =>
        await _dbContext.Propiedades.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    // NOTA: no existe un IUnitOfWork separado en los contratos de Application
    // aprobados en Fase 3 (ILeadRepository/IPropertyRepository no exponen un
    // método de commit independiente) — por eso AddAsync confirma el cambio de
    // inmediato. Es suficiente para los casos de uso actuales (una sola
    // operación por agregado); si en el futuro se necesita una transacción que
    // abarque varios agregados, se debe introducir un IUnitOfWork explícito.
    public async Task AddAsync(Propiedad propiedad, CancellationToken cancellationToken)
    {
        await _dbContext.Propiedades.AddAsync(propiedad, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
