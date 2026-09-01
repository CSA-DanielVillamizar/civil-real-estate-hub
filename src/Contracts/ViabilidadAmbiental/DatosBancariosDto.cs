namespace Plataforma.Contracts.ViabilidadAmbiental;

public sealed record DatosBancariosDto(
    string Banco,
    string TipoCuenta,
    string NumeroCuenta,
    string TitularCuenta,
    string QrImageUrl
);
