namespace Plataforma.Contracts.ViabilidadAmbiental;

public sealed record SolicitarViabilidadAmbientalRequest(
    string Nombre,
    string Email,
    string Telefono,
    string? Indicativo,
    Guid? PropiedadId,
    string? Departamento,
    string? Municipio,
    string? DireccionReferencia
);
