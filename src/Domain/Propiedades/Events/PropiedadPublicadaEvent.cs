using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.Events;

public sealed record PropiedadPublicadaEvent(PropiedadId PropiedadId) : DomainEvent;
