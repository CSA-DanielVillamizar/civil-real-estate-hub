using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.Events;

// Consumido por el Bounded Context CRM/Leads: NO descarta leads interesados,
// dispara LeadRequiereNuevaOfertaEvent (ver docs/01-domain-model.md v1.1, §5).
public sealed record PropiedadVendidaEvent(PropiedadId PropiedadId, string Municipio) : DomainEvent;
