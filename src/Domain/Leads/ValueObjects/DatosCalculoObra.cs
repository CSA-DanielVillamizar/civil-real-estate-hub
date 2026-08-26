using Plataforma.Domain.Common;

namespace Plataforma.Domain.Leads.ValueObjects;

public sealed class DatosCalculoObra : ValueObject
{
    public decimal AreaConstruccionM2 { get; }
    public TipoAcabado TipoAcabado { get; }
    public string Municipio { get; }
    public TipoProyecto TipoProyecto { get; }

    // Reservado para materialización de EF Core (Fase 4).
    private DatosCalculoObra() { }

    private DatosCalculoObra(decimal areaConstruccionM2, TipoAcabado tipoAcabado, string municipio, TipoProyecto tipoProyecto)
    {
        AreaConstruccionM2 = areaConstruccionM2;
        TipoAcabado = tipoAcabado;
        Municipio = municipio;
        TipoProyecto = tipoProyecto;
    }

    public static DatosCalculoObra Crear(decimal areaConstruccionM2, TipoAcabado tipoAcabado, string municipio, TipoProyecto tipoProyecto)
    {
        if (areaConstruccionM2 <= 0 || areaConstruccionM2 > 100_000)
            throw new ArgumentException("El área de construcción debe estar entre 0 y 100.000 m².", nameof(areaConstruccionM2));

        if (string.IsNullOrWhiteSpace(municipio))
            throw new ArgumentException("El municipio es obligatorio.", nameof(municipio));

        return new DatosCalculoObra(areaConstruccionM2, tipoAcabado, municipio.Trim(), tipoProyecto);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return AreaConstruccionM2;
        yield return TipoAcabado;
        yield return Municipio;
        yield return TipoProyecto;
    }
}
