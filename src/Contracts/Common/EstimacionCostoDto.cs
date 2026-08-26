namespace Plataforma.Contracts.Common;

public sealed record DesgloseItemDto(
    string Categoria,
    decimal Monto
);

public sealed record EstimacionCostoDto(
    decimal MontoMinimo,
    decimal MontoMaximo,
    string Moneda,
    IReadOnlyList<DesgloseItemDto> Desglose
);
