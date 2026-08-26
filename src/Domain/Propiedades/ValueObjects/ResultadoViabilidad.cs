using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.ValueObjects;

public sealed class ResultadoViabilidad : ValueObject
{
    public bool EsViable { get; }
    public IReadOnlyList<string> Restricciones { get; }

    private ResultadoViabilidad(bool esViable, IReadOnlyList<string> restricciones)
    {
        EsViable = esViable;
        Restricciones = restricciones;
    }

    public static ResultadoViabilidad Crear(bool esViable, IReadOnlyList<string> restricciones) =>
        new(esViable, restricciones);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return EsViable;
        foreach (var restriccion in Restricciones)
            yield return restriccion;
    }
}
