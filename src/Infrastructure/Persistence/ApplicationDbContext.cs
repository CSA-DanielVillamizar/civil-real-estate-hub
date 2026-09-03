using MediatR;
using Microsoft.EntityFrameworkCore;
using Plataforma.Domain.Common;
using Plataforma.Domain.Confianza;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Obras;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.Usuarios;
using Plataforma.Domain.ViabilidadAmbiental;
using Plataforma.Infrastructure.Persistence.Configurations;

namespace Plataforma.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    private readonly IPublisher _publisher;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
    }

    public DbSet<Propiedad> Propiedades => Set<Propiedad>();

    public DbSet<Lead> Leads => Set<Lead>();

    public DbSet<SolicitudViabilidadAmbiental> SolicitudesViabilidadAmbiental => Set<SolicitudViabilidadAmbiental>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<ProyectoObra> ProyectosObra => Set<ProyectoObra>();

    public DbSet<ContenidoConfianza> ContenidosConfianza => Set<ContenidoConfianza>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PropiedadConfiguration());
        modelBuilder.ApplyConfiguration(new LeadConfiguration());
        modelBuilder.ApplyConfiguration(new SolicitudViabilidadAmbientalConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
        modelBuilder.ApplyConfiguration(new ProyectoObraConfiguration());
        modelBuilder.ApplyConfiguration(new ContenidoConfianzaConfiguration());
    }

    // Prompt 4, ítem 1: los eventos de dominio se despachan ANTES de confirmar
    // la transacción. Se abre una transacción explícita, se escribe el cambio,
    // se publican los eventos y solo entonces se hace commit — si un handler
    // lanza una excepción, la escritura completa se revierte.
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await using var transaction = await Database.BeginTransactionAsync(cancellationToken);

        var result = await base.SaveChangesAsync(cancellationToken);

        var agregadosConEventos = ChangeTracker.Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity)
            .Where(entity => entity.DomainEvents.Count > 0)
            .ToList();

        var eventosPendientes = agregadosConEventos
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        foreach (var agregado in agregadosConEventos)
            agregado.ClearDomainEvents();

        foreach (var domainEvent in eventosPendientes)
            await _publisher.Publish(domainEvent, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return result;
    }
}
