using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Tarifas;

public sealed record CrearPaqueteTarifaRequest(
    ServicioDeInteresDto ServicioRelacionado,
    string Titulo,
    string Descripcion,
    decimal? PrecioDesde,
    decimal? PrecioHasta,
    string UnidadPrecio,
    string Moneda);
