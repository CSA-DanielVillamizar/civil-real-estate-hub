using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Infrastructure.Persistence.Configurations;

// Toda la configuración del agregado Propiedad — incluyendo sus miembros
// dependientes (ArchivoMultimedia, RetiroAmbiental) — vive en un único
// IEntityTypeConfiguration, reflejando el límite del aggregate: ningún tipo
// interno se expone como entidad de nivel superior con su propio DbSet.
public sealed class PropiedadConfiguration : IEntityTypeConfiguration<Propiedad>
{
    public void Configure(EntityTypeBuilder<Propiedad> builder)
    {
        builder.ToTable("propiedades");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => new PropiedadId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(p => p.Titulo)
            .HasColumnName("titulo")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Descripcion)
            .HasColumnName("descripcion")
            .IsRequired();

        builder.Property(p => p.TipoInmueble)
            .HasColumnName("tipo_inmueble")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(p => p.Estado)
            .HasColumnName("estado")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        ConfigurarPrecio(builder);
        ConfigurarUbicacion(builder);
        ConfigurarAreas(builder);
        ConfigurarCaracteristicasTopograficas(builder);
        ConfigurarRetirosAmbientales(builder);
        ConfigurarMultimedia(builder);

        builder.HasIndex(p => p.Estado);
        builder.HasIndex(p => p.TipoInmueble);
    }

    private static void ConfigurarPrecio(EntityTypeBuilder<Propiedad> builder)
    {
        builder.OwnsOne(p => p.Precio, precio =>
        {
            precio.Property(m => m.Monto)
                .HasColumnName("precio_monto")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            precio.Property(m => m.Moneda)
                .HasColumnName("precio_moneda")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Navigation(p => p.Precio).IsRequired();
    }

    private static void ConfigurarUbicacion(EntityTypeBuilder<Propiedad> builder)
    {
        builder.OwnsOne(p => p.Ubicacion, ubicacion =>
        {
            ubicacion.Property(u => u.Direccion).HasColumnName("direccion").HasMaxLength(300).IsRequired();
            ubicacion.Property(u => u.Municipio).HasColumnName("municipio").HasMaxLength(100).IsRequired();
            ubicacion.Property(u => u.Departamento).HasColumnName("departamento").HasMaxLength(100).IsRequired();

            ubicacion.OwnsOne(u => u.Coordenadas, coordenadas =>
            {
                coordenadas.Property(c => c.Latitud).HasColumnName("latitud").HasColumnType("numeric(9,6)");
                coordenadas.Property(c => c.Longitud).HasColumnName("longitud").HasColumnType("numeric(9,6)");
            });
        });

        builder.Navigation(p => p.Ubicacion).IsRequired();
    }

    private static void ConfigurarAreas(EntityTypeBuilder<Propiedad> builder)
    {
        builder.OwnsOne(p => p.AreaTerreno, area =>
        {
            area.Property(a => a.Valor).HasColumnName("area_terreno_valor").HasColumnType("numeric(12,2)").IsRequired();
            area.Property(a => a.UnidadMedida).HasColumnName("area_terreno_unidad").HasConversion<string>().HasMaxLength(20).IsRequired();
        });
        builder.Navigation(p => p.AreaTerreno).IsRequired();

        // AreaConstruida es opcional (Area?) — owned type "silencioso": si todas
        // sus columnas son NULL, EF Core materializa la propiedad como null.
        builder.OwnsOne(p => p.AreaConstruida, area =>
        {
            area.Property(a => a.Valor).HasColumnName("area_construida_valor").HasColumnType("numeric(12,2)");
            area.Property(a => a.UnidadMedida).HasColumnName("area_construida_unidad").HasConversion<string>().HasMaxLength(20);
        });
    }

    private static void ConfigurarCaracteristicasTopograficas(EntityTypeBuilder<Propiedad> builder)
    {
        builder.OwnsOne(p => p.CaracteristicasTopograficas, ct =>
        {
            ct.Property(c => c.PendientePorcentaje)
                .HasColumnName("pendiente_porcentaje")
                .HasColumnType("numeric(5,2)")
                .IsRequired();

            ct.Property(c => c.TipoSuelo)
                .HasColumnName("tipo_suelo")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            ct.Property(c => c.Topografia)
                .HasColumnName("topografia")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            ct.Property(c => c.NivelFreaticoMetros)
                .HasColumnName("nivel_freatico_metros")
                .HasColumnType("numeric(6,2)");
        });

        builder.Navigation(p => p.CaracteristicasTopograficas).IsRequired();
    }

    // RetiroAmbiental es un Value Object puro (sin identidad propia) → OwnsMany
    // con clave subrogada (shadow property "id") generada por convención.
    private static void ConfigurarRetirosAmbientales(EntityTypeBuilder<Propiedad> builder)
    {
        builder.OwnsMany(p => p.RetirosAmbientales, retiro =>
        {
            retiro.ToTable("propiedad_retiros_ambientales");
            retiro.WithOwner().HasForeignKey("propiedad_id");

            retiro.Property<int>("id").ValueGeneratedOnAdd();
            retiro.HasKey("id");

            retiro.Property(r => r.TipoFuente)
                .HasColumnName("tipo_fuente")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            retiro.Property(r => r.DistanciaMinimaMetros)
                .HasColumnName("distancia_minima_metros")
                .HasColumnType("numeric(8,2)")
                .IsRequired();

            retiro.Property(r => r.NormativaAplicable)
                .HasColumnName("normativa_aplicable")
                .HasMaxLength(300)
                .IsRequired();
        });

        // La colección expuesta es IReadOnlyCollection<T>, respaldada por el
        // campo privado _retirosAmbientales — EF debe leer/escribir por el campo.
        builder.Navigation(p => p.RetirosAmbientales).UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    // ArchivoMultimedia tiene identidad propia (Entity<Guid>), pero solo se
    // referencia desde dentro del agregado Propiedad (sin repositorio propio) →
    // se mapea igualmente como OwnsMany, usando su Id real como clave.
    private static void ConfigurarMultimedia(EntityTypeBuilder<Propiedad> builder)
    {
        builder.OwnsMany(p => p.Multimedia, multimedia =>
        {
            multimedia.ToTable("propiedad_multimedia");
            multimedia.WithOwner().HasForeignKey("propiedad_id");

            multimedia.HasKey(m => m.Id);
            multimedia.Property(m => m.Id).HasColumnName("id").ValueGeneratedNever();

            multimedia.Property(m => m.Url).HasColumnName("url").HasMaxLength(500).IsRequired();

            multimedia.Property(m => m.Tipo)
                .HasColumnName("tipo")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            multimedia.Property(m => m.Orden).HasColumnName("orden").IsRequired();
        });

        builder.Navigation(p => p.Multimedia).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
