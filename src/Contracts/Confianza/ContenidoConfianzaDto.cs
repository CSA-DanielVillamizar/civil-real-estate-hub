namespace Plataforma.Contracts.Confianza;

// DTO de salida compartido por el listado admin y el listado público — la
// única diferencia entre ambos endpoints es el filtro server-side por
// Publicado, no la forma del dato.
public sealed record ContenidoConfianzaDto(
    Guid Id,
    string Tipo,
    string Titulo,
    string Descripcion,
    string? Municipio,
    string ServicioRelacionado,
    bool Publicado,
    DateTimeOffset CreadoEn);
