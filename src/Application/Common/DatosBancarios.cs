namespace Plataforma.Application.Common;

// Datos de referencia para transferencia manual (Fase 3 — sin pasarela de
// pago). Vienen de configuración (ver IDatosBancariosProvider) y pueden
// llegar vacíos si aún no se han configurado — el llamador decide cómo
// mostrarlo (ej. "pendiente de publicar" en el frontend).
public sealed record DatosBancarios(
    string Banco,
    string TipoCuenta,
    string NumeroCuenta,
    string TitularCuenta,
    string QrImageUrl);
