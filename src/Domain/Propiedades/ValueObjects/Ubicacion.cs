using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.ValueObjects;

public sealed class Ubicacion : ValueObject
{
    public string Direccion { get; }
    public string Municipio { get; }
    public string Departamento { get; }
    public Coordenadas? Coordenadas { get; }

    // Reservado para materialización de EF Core (Fase 4).
    private Ubicacion() { }

    private Ubicacion(string direccion, string municipio, string departamento, Coordenadas? coordenadas)
    {
        Direccion = direccion;
        Municipio = municipio;
        Departamento = departamento;
        Coordenadas = coordenadas;
    }

    public static Ubicacion Crear(string direccion, string municipio, string departamento, Coordenadas? coordenadas = null)
    {
        if (string.IsNullOrWhiteSpace(direccion))
            throw new ArgumentException("La dirección es obligatoria.", nameof(direccion));

        if (string.IsNullOrWhiteSpace(municipio))
            throw new ArgumentException("El municipio es obligatorio.", nameof(municipio));

        if (string.IsNullOrWhiteSpace(departamento))
            throw new ArgumentException("El departamento es obligatorio.", nameof(departamento));

        return new Ubicacion(direccion.Trim(), municipio.Trim(), departamento.Trim(), coordenadas);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Direccion;
        yield return Municipio;
        yield return Departamento;
        yield return Coordenadas;
    }
}
