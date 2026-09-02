using Plataforma.Contracts.Obras;
using ApplicationAgregarEvidenciaHitoCommand = Plataforma.Application.Obras.Commands.AgregarEvidenciaHito.AgregarEvidenciaHitoCommand;
using ApplicationAgregarEvidenciaHitoResult = Plataforma.Application.Obras.Commands.AgregarEvidenciaHito.AgregarEvidenciaHitoResult;
using ApplicationAgregarHitoCommand = Plataforma.Application.Obras.Commands.AgregarHito.AgregarHitoCommand;
using ApplicationAgregarHitoResult = Plataforma.Application.Obras.Commands.AgregarHito.AgregarHitoResult;
using ApplicationCambiarEstadoHitoCommand = Plataforma.Application.Obras.Commands.CambiarEstadoHito.CambiarEstadoHitoCommand;
using ApplicationCambiarEstadoHitoResult = Plataforma.Application.Obras.Commands.CambiarEstadoHito.CambiarEstadoHitoResult;
using ApplicationCambiarEstadoProyectoCommand = Plataforma.Application.Obras.Commands.CambiarEstadoProyecto.CambiarEstadoProyectoCommand;
using ApplicationCambiarEstadoProyectoResult = Plataforma.Application.Obras.Commands.CambiarEstadoProyecto.CambiarEstadoProyectoResult;
using ApplicationCrearProyectoObraCommand = Plataforma.Application.Obras.Commands.CrearProyectoObra.CrearProyectoObraCommand;
using ApplicationCrearProyectoObraResult = Plataforma.Application.Obras.Commands.CrearProyectoObra.CrearProyectoObraResult;
using ApplicationHitoItem = Plataforma.Application.Obras.Queries.Common.HitoItem;
using ApplicationProyectoObraDetalle = Plataforma.Application.Obras.Queries.Common.ProyectoObraDetalle;
using ApplicationProyectoObraListItem = Plataforma.Application.Obras.Queries.ObtenerProyectosObra.ProyectoObraListItem;

namespace Plataforma.WebApi.Mapping;

public static class ObrasMapping
{
    public static ApplicationCrearProyectoObraCommand ToCommand(this CrearProyectoObraRequest request) => new(
        request.NombreCliente,
        request.EmailCliente,
        request.TelefonoCliente,
        request.IndicativoCliente,
        request.NombreProyecto,
        request.Descripcion,
        request.PropiedadId);

    public static CrearProyectoObraResponse ToContract(this ApplicationCrearProyectoObraResult result) =>
        new(result.Id, result.TokenAcceso);

    public static ApplicationAgregarHitoCommand ToCommand(this AgregarHitoRequest request, Guid proyectoObraId) =>
        new(proyectoObraId, request.Nombre, request.Descripcion, request.FechaEstimada);

    public static HitoResponse ToContract(this ApplicationAgregarHitoResult result) =>
        new(result.HitoId, result.Nombre, result.Estado);

    public static ApplicationCambiarEstadoHitoCommand ToCommand(this CambiarEstadoHitoRequest request, Guid proyectoObraId, Guid hitoId) =>
        new(proyectoObraId, hitoId, request.NuevoEstado.ToDomain());

    public static EstadoHitoResponse ToContract(this ApplicationCambiarEstadoHitoResult result) =>
        new(result.HitoId, result.Estado);

    public static ApplicationCambiarEstadoProyectoCommand ToCommand(this CambiarEstadoProyectoRequest request, Guid proyectoObraId) =>
        new(proyectoObraId, request.NuevoEstado.ToDomain());

    public static EstadoProyectoResponse ToContract(this ApplicationCambiarEstadoProyectoResult result) =>
        new(result.Id, result.Estado);

    public static AgregarEvidenciaHitoResponse ToContract(this ApplicationAgregarEvidenciaHitoResult result) =>
        new(result.HitoId, result.FotoEvidenciaUrl);

    public static ProyectoObraListItemDto ToContract(this ApplicationProyectoObraListItem item) => new(
        item.Id,
        item.NombreCliente,
        item.NombreProyecto,
        item.Estado.ToContract(),
        item.CreadoEn,
        item.TotalHitos,
        item.HitosCompletados,
        item.TokenAcceso);

    public static HitoDto ToContract(this ApplicationHitoItem item) => new(
        item.Id,
        item.Nombre,
        item.Descripcion,
        item.Orden,
        item.Estado.ToContract(),
        item.FechaEstimada,
        item.FechaCompletado,
        item.FotoEvidenciaUrl);

    public static ProyectoObraDetalleDto ToContract(this ApplicationProyectoObraDetalle detalle) => new(
        detalle.Id,
        detalle.NombreCliente,
        detalle.EmailCliente,
        detalle.TelefonoCliente,
        detalle.NombreProyecto,
        detalle.Descripcion,
        detalle.PropiedadId,
        detalle.Estado.ToContract(),
        detalle.CreadoEn,
        detalle.Hitos.Select(h => h.ToContract()).ToList());
}
