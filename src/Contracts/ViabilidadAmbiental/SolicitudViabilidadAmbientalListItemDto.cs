namespace Plataforma.Contracts.ViabilidadAmbiental;

public sealed record SolicitudViabilidadAmbientalListItemDto(
    Guid Id,
    string Nombre,
    string Email,
    string Telefono,
    Guid? PropiedadId,
    string? Municipio,
    string? Departamento,
    decimal Monto,
    string Moneda,
    string Estado,
    DateTimeOffset SolicitadaEn,
    DateTimeOffset? PagoConfirmadoEn);
