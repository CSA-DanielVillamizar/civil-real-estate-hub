using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Tarifas;

public sealed record ActualizarPaqueteTarifaRequest(
    string Titulo,
    string Descripcion,
    decimal? PrecioDesde,
    decimal? PrecioHasta,
    string UnidadPrecio,
    ServicioDeInteresDto ServicioRelacionado);
