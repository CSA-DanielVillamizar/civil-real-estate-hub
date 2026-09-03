using Plataforma.Domain.Tarifas;

namespace Plataforma.Application.Tarifas.Commands.Common;

public sealed record PaqueteTarifaResult(
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

public static class PaqueteTarifaMapping
{
    public static PaqueteTarifaResult ToResult(this PaqueteTarifa paquete) =>
        new(
            paquete.Id.Value,
            paquete.ServicioRelacionado.ToString(),
            paquete.Titulo,
            paquete.Descripcion,
            paquete.PrecioDesde,
            paquete.PrecioHasta,
            paquete.UnidadPrecio,
            paquete.Moneda,
            paquete.Publicado,
            paquete.CreadoEn);
}
