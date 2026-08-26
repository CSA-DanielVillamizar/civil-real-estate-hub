using Plataforma.Domain.Common;
using Plataforma.Domain.Leads.ValueObjects;

namespace Plataforma.Domain.Leads.Events;

// Se dispara aunque el usuario no deje datos de contacto (docs/01-domain-model.md, §3.4)
// — por eso NO cuelga de la raíz Lead, sino que la publica directamente el caso de uso
// de /api/budgets/calculate (fuera del alcance de esta fase; ver Prompt 3, ítem 3).
public sealed record CalculoObraRealizadoEvent(DatosCalculoObra DatosEntrada, EstimacionCosto Resultado) : DomainEvent;
