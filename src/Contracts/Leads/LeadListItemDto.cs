using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Leads;

public sealed record LeadListItemDto(
    Guid Id,
    string Nombre,
    string Email,
    string Telefono,
    OrigenLeadDto Origen,
    EstadoLeadDto Estado,
    DateTimeOffset CapturadoEn,
    Guid? PropiedadDeInteresId,
    decimal? EstimacionMontoMinimo,
    decimal? EstimacionMontoMaximo,
    string? EstimacionMoneda,
    ServicioDeInteresDto? ServicioDeInteres,
    string? Mensaje);
