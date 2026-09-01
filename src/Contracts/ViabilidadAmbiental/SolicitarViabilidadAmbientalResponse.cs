namespace Plataforma.Contracts.ViabilidadAmbiental;

public sealed record SolicitarViabilidadAmbientalResponse(
    Guid Id,
    string Estado,
    decimal Monto,
    string Moneda,
    DatosBancariosDto DatosBancarios
);
