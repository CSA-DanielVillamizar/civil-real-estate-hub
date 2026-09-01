namespace Plataforma.Application.ViabilidadAmbiental.Queries.ObtenerSolicitudesViabilidadAmbiental;

public sealed record SolicitudViabilidadAmbientalListItem(
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
