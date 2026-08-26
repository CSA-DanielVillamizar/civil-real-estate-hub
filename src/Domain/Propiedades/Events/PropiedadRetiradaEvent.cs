using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.Events;

public sealed record PropiedadRetiradaEvent(PropiedadId PropiedadId) : DomainEvent;
