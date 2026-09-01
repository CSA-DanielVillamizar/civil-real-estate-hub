using Plataforma.Domain.SharedKernel;

namespace Plataforma.Domain.ViabilidadAmbiental.Services;

// ============================================================================
// ASUNCIÓN — MONTO PLACEHOLDER, NO VALIDADO COMERCIALMENTE.
// Mismo criterio que Leads.Services.TarifarioObra: valor ilustrativo para que
// el flujo sea funcional y testeable end-to-end, sugerido por el asistente
// (dictamen técnico de escritorio, sin visita a terreno) y pendiente de que
// el negocio lo confirme o ajuste. Cambiarlo es editar esta constante, sin
// tocar el resto del dominio.
// ============================================================================
public static class TarifarioViabilidadAmbiental
{
    public static Dinero MontoEstudio() => Dinero.Crear(200_000m);
}
