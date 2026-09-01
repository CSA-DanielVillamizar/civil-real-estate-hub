using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.ViabilidadAmbiental;

namespace Plataforma.Infrastructure.Persistence.Configurations;

public sealed class SolicitudViabilidadAmbientalConfiguration : IEntityTypeConfiguration<SolicitudViabilidadAmbiental>
{
    public void Configure(EntityTypeBuilder<SolicitudViabilidadAmbiental> builder)
    {
        builder.ToTable("solicitudes_viabilidad_ambiental");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, value => new SolicitudViabilidadAmbientalId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(s => s.Estado)
            .HasColumnName("estado")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.PagoConfirmadoEn)
            .HasColumnName("pago_confirmado_en");

        // PropiedadId es una referencia débil entre bounded contexts, igual
        // que en LeadConfiguration — Guid plano, nunca FK real.
        builder.Property(s => s.PropiedadId)
            .HasColumnName("propiedad_id")
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new PropiedadId(value.Value) : (PropiedadId?)null);

        builder.OwnsOne(s => s.Solicitante, solicitante =>
        {
            solicitante.Property(d => d.Nombre).HasColumnName("solicitante_nombre").HasMaxLength(150).IsRequired();

            solicitante.OwnsOne(d => d.Email, email =>
            {
                email.Property(e => e.Valor).HasColumnName("solicitante_email").HasMaxLength(254).IsRequired();
            });
            solicitante.Navigation(d => d.Email).IsRequired();

            solicitante.OwnsOne(d => d.Telefono, telefono =>
            {
                telefono.Property(t => t.Numero).HasColumnName("solicitante_telefono_numero").HasMaxLength(15).IsRequired();
                telefono.Property(t => t.Indicativo).HasColumnName("solicitante_telefono_indicativo").HasMaxLength(5).IsRequired();
            });
            solicitante.Navigation(d => d.Telefono).IsRequired();
        });
        builder.Navigation(s => s.Solicitante).IsRequired();

        // Opcional: solo presente cuando no hay PropiedadId. Departamento y
        // Municipio son NOT NULL cuando el owned type existe (garantizado por
        // UbicacionLote.Crear), así que EF distingue "sin ubicación" de
        // "ubicación con columnas nulas" sin necesitar un scalar extra como
        // EstimacionCosto.CalculadoEn.
        builder.OwnsOne(s => s.UbicacionLote, ubicacion =>
        {
            ubicacion.Property(u => u.Departamento).HasColumnName("lote_departamento").HasMaxLength(100);
            ubicacion.Property(u => u.Municipio).HasColumnName("lote_municipio").HasMaxLength(100);
            ubicacion.Property(u => u.DireccionReferencia).HasColumnName("lote_direccion_referencia").HasMaxLength(250);
        });

        builder.OwnsOne(s => s.Monto, monto =>
        {
            monto.Property(m => m.Monto).HasColumnName("monto").HasColumnType("numeric(18,2)").IsRequired();
            monto.Property(m => m.Moneda).HasColumnName("moneda").HasMaxLength(3).IsRequired();
        });
        builder.Navigation(s => s.Monto).IsRequired();

        builder.HasIndex(s => s.Estado);
    }
}
