using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Domain.Tarifas;

namespace Plataforma.Infrastructure.Persistence.Configurations;

public sealed class PaqueteTarifaConfiguration : IEntityTypeConfiguration<PaqueteTarifa>
{
    public void Configure(EntityTypeBuilder<PaqueteTarifa> builder)
    {
        builder.ToTable("paquetes_tarifa");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => new PaqueteTarifaId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(p => p.ServicioRelacionado)
            .HasColumnName("servicio_relacionado")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Titulo)
            .HasColumnName("titulo")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(p => p.PrecioDesde)
            .HasColumnName("precio_desde")
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.PrecioHasta)
            .HasColumnName("precio_hasta")
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.UnidadPrecio)
            .HasColumnName("unidad_precio")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Moneda)
            .HasColumnName("moneda")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(p => p.Publicado)
            .HasColumnName("publicado")
            .IsRequired();

        builder.Property(p => p.CreadoEn)
            .HasColumnName("creado_en")
            .IsRequired();
    }
}
