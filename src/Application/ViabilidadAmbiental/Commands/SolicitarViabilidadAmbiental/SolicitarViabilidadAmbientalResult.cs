using Plataforma.Application.Common;

namespace Plataforma.Application.ViabilidadAmbiental.Commands.SolicitarViabilidadAmbiental;

public sealed record SolicitarViabilidadAmbientalResult(
    Guid Id,
    string Estado,
    decimal Monto,
    string Moneda,
    DatosBancarios DatosBancarios);
