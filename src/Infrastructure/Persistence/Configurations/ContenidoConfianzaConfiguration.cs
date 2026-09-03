using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Domain.Confianza;

namespace Plataforma.Infrastructure.Persistence.Configurations;

public sealed class ContenidoConfianzaConfiguration : IEntityTypeConfiguration<ContenidoConfianza>
{
    public void Configure(EntityTypeBuilder<ContenidoConfianza> builder)
    {
        builder.ToTable("contenidos_confianza");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => new ContenidoConfianzaId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(c => c.Tipo)
            .HasColumnName("tipo")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Titulo)
            .HasColumnName("titulo")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(c => c.Municipio)
            .HasColumnName("municipio")
            .HasMaxLength(100);

        builder.Property(c => c.ServicioRelacionado)
            .HasColumnName("servicio_relacionado")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Publicado)
            .HasColumnName("publicado")
            .IsRequired();

        builder.Property(c => c.CreadoEn)
            .HasColumnName("creado_en")
            .IsRequired();
    }
}
