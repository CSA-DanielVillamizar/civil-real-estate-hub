namespace Plataforma.Contracts.ViabilidadAmbiental;

public sealed record ConfirmarPagoViabilidadAmbientalResponse(
    Guid Id,
    string Estado,
    DateTimeOffset PagoConfirmadoEn
);
