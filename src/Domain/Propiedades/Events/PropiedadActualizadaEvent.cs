using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.Events;

public sealed record PropiedadActualizadaEvent(PropiedadId PropiedadId) : DomainEvent;
