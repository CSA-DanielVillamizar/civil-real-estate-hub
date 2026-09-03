using Plataforma.Application.Tarifas.Commands.ActualizarPaqueteTarifa;
using Plataforma.Application.Tarifas.Commands.Common;
using Plataforma.Application.Tarifas.Commands.CrearPaqueteTarifa;
using Plataforma.Contracts.Tarifas;

namespace Plataforma.WebApi.Mapping;

public static class TarifasMapping
{
    public static CrearPaqueteTarifaCommand ToCommand(this CrearPaqueteTarifaRequest request) =>
        new(
            request.ServicioRelacionado.ToDomain(), request.Titulo, request.Descripcion,
            request.PrecioDesde, request.PrecioHasta, request.UnidadPrecio, request.Moneda);

    public static ActualizarPaqueteTarifaCommand ToCommand(this ActualizarPaqueteTarifaRequest request, Guid paqueteId) =>
        new(
            paqueteId, request.Titulo, request.Descripcion,
            request.PrecioDesde, request.PrecioHasta, request.UnidadPrecio, request.ServicioRelacionado.ToDomain());

    public static PaqueteTarifaDto ToContract(this PaqueteTarifaResult result) =>
        new(
            result.Id, result.ServicioRelacionado, result.Titulo, result.Descripcion,
            result.PrecioDesde, result.PrecioHasta, result.UnidadPrecio, result.Moneda, result.Publicado, result.CreadoEn);
}
