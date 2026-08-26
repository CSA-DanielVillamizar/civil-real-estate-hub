using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.ValueObjects;

public sealed class Coordenadas : ValueObject
{
    public decimal Latitud { get; }
    public decimal Longitud { get; }

    // Reservado para materialización de EF Core (Fase 4).
    private Coordenadas() { }

    private Coordenadas(decimal latitud, decimal longitud)
    {
        Latitud = latitud;
        Longitud = longitud;
    }

    public static Coordenadas Crear(decimal latitud, decimal longitud)
    {
        if (latitud is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitud), "La latitud debe estar entre -90 y 90.");

        if (longitud is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitud), "La longitud debe estar entre -180 y 180.");

        return new Coordenadas(latitud, longitud);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Latitud;
        yield return Longitud;
    }
}
