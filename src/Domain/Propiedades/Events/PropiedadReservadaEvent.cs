using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.Events;

public sealed record PropiedadReservadaEvent(PropiedadId PropiedadId) : DomainEvent;
