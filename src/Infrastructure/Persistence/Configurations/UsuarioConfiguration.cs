using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Domain.Usuarios;

namespace Plataforma.Infrastructure.Persistence.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(id => id.Value, value => new UsuarioId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(u => u.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(u => u.Rol)
            .HasColumnName("rol")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(u => u.Activo)
            .HasColumnName("activo")
            .IsRequired();

        builder.Property(u => u.CreadoEn)
            .HasColumnName("creado_en")
            .IsRequired();

        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Valor).HasColumnName("email").HasMaxLength(254).IsRequired();
            email.HasIndex(e => e.Valor).IsUnique();
        });
        builder.Navigation(u => u.Email).IsRequired();
    }
}
