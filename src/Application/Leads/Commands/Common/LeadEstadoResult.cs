namespace Plataforma.Application.Leads.Commands.Common;

// Compartido por las 4 transiciones administrativas (Contactado, Calificar,
// Convertir, Descartar) — todas devuelven exactamente esta forma.
public sealed record LeadEstadoResult(Guid Id, string Estado);
