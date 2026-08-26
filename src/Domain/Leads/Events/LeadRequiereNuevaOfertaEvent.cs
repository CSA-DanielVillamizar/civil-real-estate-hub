using Plataforma.Domain.Common;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Domain.Leads.Events;

// Handler suscrito a PropiedadVendidaEvent (Inmobiliaria) — ver docs/01-domain-model.md v1.1, §5.
// La suscripción cross-context se conecta en la capa de Infraestructura/Application (Fase 4).
public sealed record LeadRequiereNuevaOfertaEvent(LeadId LeadId, PropiedadId PropiedadVendidaId, string Municipio) : DomainEvent;
