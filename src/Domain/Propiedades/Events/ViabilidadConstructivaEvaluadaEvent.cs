using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.Events;

public sealed record ViabilidadConstructivaEvaluadaEvent(
    PropiedadId PropiedadId,
    bool EsViable,
    IReadOnlyList<string> Restricciones
) : DomainEvent;
