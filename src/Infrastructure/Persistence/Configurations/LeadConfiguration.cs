using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Infrastructure.Persistence.Configurations;

public sealed class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable("leads");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .HasConversion(id => id.Value, value => new LeadId(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(l => l.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(l => l.Origen)
            .HasColumnName("origen")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(l => l.Estado)
            .HasColumnName("estado")
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        // PropiedadId es un value object externo (referencia débil entre bounded
        // contexts — ver docs/01-domain-model.md §5): se guarda como Guid plano,
        // nunca como FK real hacia la tabla propiedades.
        builder.Property(l => l.PropiedadDeInteresId)
            .HasColumnName("propiedad_de_interes_id")
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new PropiedadId(value.Value) : (PropiedadId?)null);

        builder.OwnsOne(l => l.Email, email =>
        {
            email.Property(e => e.Valor).HasColumnName("email").HasMaxLength(254).IsRequired();
        });
        builder.Navigation(l => l.Email).IsRequired();

        builder.OwnsOne(l => l.Telefono, telefono =>
        {
            telefono.Property(t => t.Numero).HasColumnName("telefono_numero").HasMaxLength(15).IsRequired();
            telefono.Property(t => t.Indicativo).HasColumnName("telefono_indicativo").HasMaxLength(5).IsRequired();
        });
        builder.Navigation(l => l.Telefono).IsRequired();

        ConfigurarResultadoCalculadora(builder);

        builder.HasIndex(l => l.Estado);
        builder.HasIndex(l => l.Origen);
    }

    // Snapshot inmutable congelado en el momento de la captación (ver Fase 1
    // §3.1) — se mapea completo como owned type opcional, incluyendo el desglose
    // como colección owned anidada.
    private static void ConfigurarResultadoCalculadora(EntityTypeBuilder<Lead> builder)
    {
        builder.OwnsOne(l => l.ResultadoCalculadora, estimacion =>
        {
            // Propiedad escalar propia y requerida — permite a EF distinguir
            // "no hay estimación" de "todas las columnas anidadas son NULL"
            // (ver EstimacionCosto.CalculadoEn).
            estimacion.Property(e => e.CalculadoEn)
                .HasColumnName("estimacion_calculada_en")
                .IsRequired();

            estimacion.OwnsOne(e => e.MontoMinimo, monto =>
            {
                monto.Property(m => m.Monto).HasColumnName("estimacion_minima_monto").HasColumnType("numeric(18,2)");
                monto.Property(m => m.Moneda).HasColumnName("estimacion_minima_moneda").HasMaxLength(3);
            });

            estimacion.OwnsOne(e => e.MontoMaximo, monto =>
            {
                monto.Property(m => m.Monto).HasColumnName("estimacion_maxima_monto").HasColumnType("numeric(18,2)");
                monto.Property(m => m.Moneda).HasColumnName("estimacion_maxima_moneda").HasMaxLength(3);
            });

            estimacion.OwnsOne(e => e.DatosEntrada, datos =>
            {
                datos.Property(d => d.AreaConstruccionM2).HasColumnName("calculo_area_construccion_m2").HasColumnType("numeric(12,2)");
                datos.Property(d => d.TipoAcabado).HasColumnName("calculo_tipo_acabado").HasConversion<string>().HasMaxLength(20);
                datos.Property(d => d.Municipio).HasColumnName("calculo_municipio").HasMaxLength(100);
                datos.Property(d => d.TipoProyecto).HasColumnName("calculo_tipo_proyecto").HasConversion<string>().HasMaxLength(20);
            });

            estimacion.OwnsMany(e => e.Desglose, desglose =>
            {
                desglose.ToTable("lead_estimacion_desglose");
                desglose.WithOwner().HasForeignKey("lead_id");

                desglose.Property<int>("id").ValueGeneratedOnAdd();
                desglose.HasKey("id");

                desglose.Property(d => d.Categoria).HasColumnName("categoria").HasMaxLength(60).IsRequired();

                desglose.OwnsOne(d => d.Monto, monto =>
                {
                    monto.Property(m => m.Monto).HasColumnName("monto").HasColumnType("numeric(18,2)").IsRequired();
                    monto.Property(m => m.Moneda).HasColumnName("moneda").HasMaxLength(3).IsRequired();
                });
            });

            estimacion.Navigation(e => e.Desglose).UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}
