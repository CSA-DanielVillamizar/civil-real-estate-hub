namespace Plataforma.Domain.ViabilidadAmbiental;

// Fase 3 (SDD): sin pasarela de pago — el cliente transfiere directamente a la
// cuenta bancaria publicada y un administrador confirma manualmente. Por eso
// el ciclo de vida se detiene en Pagada/Rechazada; no hay EnRevisión/Completada
// todavía porque la entrega del informe técnico sigue siendo un proceso manual
// fuera de la plataforma (igual que la asignación de consultor comercial hoy).
public enum EstadoSolicitudViabilidad
{
    Solicitada,
    Pagada,
    Rechazada,
}
