using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Domain.Obras;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Infrastructure.Persistence.Configurations;

public sealed class ProyectoObraConfiguration : IEntityTypeConfiguration<ProyectoObra>
{
    public void Configure(EntityTypeBuilder<ProyectoObra> builder)
    {
        builder.ToTable("proyectos_obra");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => new ProyectoObraId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(p => p.NombreCliente).HasColumnName("nombre_cliente").HasMaxLength(150).IsRequired();
        builder.Property(p => p.NombreProyecto).HasColumnName("nombre_proyecto").HasMaxLength(200).IsRequired();
        builder.Property(p => p.Descripcion).HasColumnName("descripcion").HasMaxLength(2000);

        builder.Property(p => p.TokenAcceso).HasColumnName("token_acceso").HasMaxLength(64).IsRequired();
        // Es la única credencial de acceso del cliente — buscada por
        // GetByTokenAsync en cada visita a /mi-obra/{token}, y debe ser único.
        builder.HasIndex(p => p.TokenAcceso).IsUnique();

        builder.Property(p => p.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(p => p.CreadoEn).HasColumnName("creado_en").IsRequired();

        // Referencia débil entre bounded contexts (igual que
        // Lead.PropiedadDeInteresId) — Guid plano, nunca FK real.
        builder.Property(p => p.PropiedadId)
            .HasColumnName("propiedad_id")
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new PropiedadId(value.Value) : (PropiedadId?)null);

        builder.OwnsOne(p => p.EmailCliente, email =>
        {
            email.Property(e => e.Valor).HasColumnName("email_cliente").HasMaxLength(254).IsRequired();
        });
        builder.Navigation(p => p.EmailCliente).IsRequired();

        builder.OwnsOne(p => p.TelefonoCliente, telefono =>
        {
            telefono.Property(t => t.Numero).HasColumnName("telefono_cliente_numero").HasMaxLength(15).IsRequired();
            telefono.Property(t => t.Indicativo).HasColumnName("telefono_cliente_indicativo").HasMaxLength(5).IsRequired();
        });
        builder.Navigation(p => p.TelefonoCliente).IsRequired();

        // HitoObra tiene identidad propia (Entity<Guid>) pero solo se accede
        // a través del agregado ProyectoObra (sin repositorio propio) — mismo
        // patrón que Propiedad.Multimedia (ver PropiedadConfiguration).
        builder.OwnsMany(p => p.Hitos, hito =>
        {
            hito.ToTable("hitos_obra");
            hito.WithOwner().HasForeignKey("proyecto_obra_id");

            hito.HasKey(h => h.Id);
            hito.Property(h => h.Id).HasColumnName("id").ValueGeneratedNever();

            hito.Property(h => h.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
            hito.Property(h => h.Descripcion).HasColumnName("descripcion").HasMaxLength(2000);
            hito.Property(h => h.Orden).HasColumnName("orden").IsRequired();
            hito.Property(h => h.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20).IsRequired();
            hito.Property(h => h.FechaEstimada).HasColumnName("fecha_estimada");
            hito.Property(h => h.FechaCompletado).HasColumnName("fecha_completado");
            hito.Property(h => h.FotoEvidenciaUrl).HasColumnName("foto_evidencia_url").HasMaxLength(500);
        });

        builder.Navigation(p => p.Hitos).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
