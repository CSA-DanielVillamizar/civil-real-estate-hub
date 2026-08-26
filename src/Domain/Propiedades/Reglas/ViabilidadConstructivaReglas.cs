namespace Plataforma.Domain.Propiedades.Reglas;

// ASUNCIÓN PENDIENTE DE VALIDACIÓN NORMATIVA:
// El umbral de pendiente máxima es un valor de referencia general de urbanismo
// (POT), NO un dato confirmado por el negocio ni por una autoridad municipal
// específica. Debe validarse antes de usarse para tomar decisiones reales de
// viabilidad constructiva. Este motor de reglas es intencionalmente simple
// (Fase 1, §1.4): solo evalúa pendiente y deja constancia informativa de los
// retiros ambientales registrados, sin bloquear la viabilidad por su sola
// presencia (no hay dato de "distancia real construida" para contrastar contra
// el retiro mínimo exigido).
public static class ViabilidadConstructivaReglas
{
    public const decimal PendienteMaximaPermitidaPorcentaje = 25m;
}
