using Plataforma.Domain.Common;

namespace Plataforma.Domain.ViabilidadAmbiental.ValueObjects;

// Solo se usa cuando el solicitante todavía no tiene el lote registrado como
// Propiedad en el catálogo (ver SolicitudViabilidadAmbiental — es mutuamente
// excluyente con PropiedadId). Deliberadamente sin Coordenadas/VOs de
// Propiedades: es una referencia libre dada por el cliente al pedir el
// estudio, no un dato validado del catálogo.
public sealed class UbicacionLote : ValueObject
{
    public string Departamento { get; }
    public string Municipio { get; }
    public string? DireccionReferencia { get; }

    // Reservado para materialización de EF Core.
    private UbicacionLote() { }

    private UbicacionLote(string departamento, string municipio, string? direccionReferencia)
    {
        Departamento = departamento;
        Municipio = municipio;
        DireccionReferencia = direccionReferencia;
    }

    public static UbicacionLote Crear(string departamento, string municipio, string? direccionReferencia = null)
    {
        if (string.IsNullOrWhiteSpace(departamento))
            throw new ArgumentException("El departamento es obligatorio.", nameof(departamento));

        if (string.IsNullOrWhiteSpace(municipio))
            throw new ArgumentException("El municipio es obligatorio.", nameof(municipio));

        return new UbicacionLote(
            departamento.Trim(),
            municipio.Trim(),
            string.IsNullOrWhiteSpace(direccionReferencia) ? null : direccionReferencia.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Departamento;
        yield return Municipio;
        yield return DireccionReferencia;
    }

    public override string ToString() => $"{Municipio}, {Departamento}";
}
