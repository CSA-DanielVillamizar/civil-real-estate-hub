namespace Plataforma.Contracts.Tarifas;

public sealed record PaqueteTarifaDto(
    Guid Id,
    string ServicioRelacionado,
    string Titulo,
    string Descripcion,
    decimal? PrecioDesde,
    decimal? PrecioHasta,
    string UnidadPrecio,
    string Moneda,
    bool Publicado,
    DateTimeOffset CreadoEn);
