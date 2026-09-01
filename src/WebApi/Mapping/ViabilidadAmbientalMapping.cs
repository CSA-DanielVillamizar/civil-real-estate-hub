using Plataforma.Application.Common;
using Plataforma.Contracts.ViabilidadAmbiental;
using ApplicationConfirmarPagoCommand = Plataforma.Application.ViabilidadAmbiental.Commands.ConfirmarPagoViabilidadAmbiental.ConfirmarPagoViabilidadAmbientalCommand;
using ApplicationConfirmarPagoResult = Plataforma.Application.ViabilidadAmbiental.Commands.ConfirmarPagoViabilidadAmbiental.ConfirmarPagoViabilidadAmbientalResult;
using ApplicationListItem = Plataforma.Application.ViabilidadAmbiental.Queries.ObtenerSolicitudesViabilidadAmbiental.SolicitudViabilidadAmbientalListItem;
using ApplicationObtenerQuery = Plataforma.Application.ViabilidadAmbiental.Queries.ObtenerSolicitudesViabilidadAmbiental.ObtenerSolicitudesViabilidadAmbientalQuery;
using ApplicationSolicitarCommand = Plataforma.Application.ViabilidadAmbiental.Commands.SolicitarViabilidadAmbiental.SolicitarViabilidadAmbientalCommand;
using ApplicationSolicitarResult = Plataforma.Application.ViabilidadAmbiental.Commands.SolicitarViabilidadAmbiental.SolicitarViabilidadAmbientalResult;

namespace Plataforma.WebApi.Mapping;

public static class ViabilidadAmbientalMapping
{
    public static ApplicationSolicitarCommand ToCommand(this SolicitarViabilidadAmbientalRequest request) => new(
        request.Nombre,
        request.Email,
        request.Telefono,
        request.Indicativo,
        request.PropiedadId,
        request.Departamento,
        request.Municipio,
        request.DireccionReferencia);

    public static SolicitarViabilidadAmbientalResponse ToContract(this ApplicationSolicitarResult result) => new(
        result.Id,
        result.Estado,
        result.Monto,
        result.Moneda,
        result.DatosBancarios.ToContract());

    public static ApplicationConfirmarPagoCommand ToConfirmarPagoCommand(this Guid solicitudId) => new(solicitudId);

    public static ConfirmarPagoViabilidadAmbientalResponse ToContract(this ApplicationConfirmarPagoResult result) => new(
        result.Id,
        result.Estado,
        result.PagoConfirmadoEn);

    public static ApplicationObtenerQuery ToApplicationQuery(this GetSolicitudesViabilidadAmbientalQuery query) => new(
        query.Estado?.ToDomain());

    public static SolicitudViabilidadAmbientalListItemDto ToContract(this ApplicationListItem item) => new(
        item.Id,
        item.Nombre,
        item.Email,
        item.Telefono,
        item.PropiedadId,
        item.Municipio,
        item.Departamento,
        item.Monto,
        item.Moneda,
        item.Estado,
        item.SolicitadaEn,
        item.PagoConfirmadoEn);

    private static DatosBancariosDto ToContract(this DatosBancarios datosBancarios) => new(
        datosBancarios.Banco,
        datosBancarios.TipoCuenta,
        datosBancarios.NumeroCuenta,
        datosBancarios.TitularCuenta,
        datosBancarios.QrImageUrl);
}
