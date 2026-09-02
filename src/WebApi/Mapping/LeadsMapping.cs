using Plataforma.Contracts.Leads;
using ApplicationCalificarLeadCommand = Plataforma.Application.Leads.Commands.CalificarLead.CalificarLeadCommand;
using ApplicationConvertirLeadCommand = Plataforma.Application.Leads.Commands.ConvertirLead.ConvertirLeadCommand;
using ApplicationCreateLeadCommand = Plataforma.Application.Leads.Commands.CreateLead.CreateLeadCommand;
using ApplicationCreateLeadResult = Plataforma.Application.Leads.Commands.CreateLead.CreateLeadResult;
using ApplicationDatosCalculoObraInput = Plataforma.Application.Leads.Commands.CreateLead.DatosCalculoObraInput;
using ApplicationDescartarLeadCommand = Plataforma.Application.Leads.Commands.DescartarLead.DescartarLeadCommand;
using ApplicationGenerarPresupuestoPdfCommand = Plataforma.Application.Leads.Commands.GenerarPresupuestoPdf.GenerarPresupuestoPdfCommand;
using ApplicationLeadEstadoResult = Plataforma.Application.Leads.Commands.Common.LeadEstadoResult;
using ApplicationLeadListItem = Plataforma.Application.Leads.Queries.ObtenerLeads.LeadListItem;
using ApplicationMarcarLeadContactadoCommand = Plataforma.Application.Leads.Commands.MarcarLeadContactado.MarcarLeadContactadoCommand;
using ApplicationObtenerLeadsQuery = Plataforma.Application.Leads.Queries.ObtenerLeads.ObtenerLeadsQuery;

namespace Plataforma.WebApi.Mapping;

public static class LeadsMapping
{
    public static ApplicationCreateLeadCommand ToCommand(this CreateLeadRequest request) => new(
        request.Nombre,
        request.Email,
        request.Telefono,
        request.Indicativo,
        request.Origen.ToDomain(),
        request.PropiedadDeInteresId,
        request.DatosCalculoObra.ToApplicationInput(),
        request.ServicioDeInteres?.ToDomain(),
        request.Mensaje);

    // Reutiliza el mismo CreateLeadRequest como body del endpoint de PDF —
    // misma forma de datos (nombre/email/telefono/datosCalculoObra), sin
    // duplicar un Contract casi idéntico. El campo "origen" del request se
    // ignora aquí: el propio endpoint implica CalculadoraObra.
    public static ApplicationGenerarPresupuestoPdfCommand ToGenerarPresupuestoPdfCommand(this CreateLeadRequest request) => new(
        request.Nombre,
        request.Email,
        request.Telefono,
        request.Indicativo,
        request.PropiedadDeInteresId,
        request.DatosCalculoObra.ToApplicationInput());

    private static ApplicationDatosCalculoObraInput? ToApplicationInput(this Plataforma.Contracts.Common.DatosCalculoObraDto? dto) =>
        dto is null
            ? null
            : new ApplicationDatosCalculoObraInput(
                dto.AreaConstruccionM2,
                dto.TipoAcabado.ToDomain(),
                dto.Municipio,
                dto.TipoProyecto.ToDomain());

    public static CreateLeadResponse ToContract(this ApplicationCreateLeadResult result) => new(
        result.Id,
        result.Estado,
        result.EstimacionCosto?.ToContract());

    public static ApplicationObtenerLeadsQuery ToApplicationQuery(this GetLeadsQuery query) => new(query.Estado?.ToDomain());

    public static LeadListItemDto ToContract(this ApplicationLeadListItem item) => new(
        item.Id,
        item.Nombre,
        item.Email,
        item.Telefono,
        item.Origen.ToContract(),
        item.Estado.ToContract(),
        item.CapturadoEn,
        item.PropiedadDeInteresId,
        item.EstimacionMontoMinimo,
        item.EstimacionMontoMaximo,
        item.EstimacionMoneda,
        item.ServicioDeInteres?.ToContract(),
        item.Mensaje);

    public static ApplicationMarcarLeadContactadoCommand ToMarcarContactadoCommand(this Guid leadId) => new(leadId);

    public static ApplicationCalificarLeadCommand ToCalificarCommand(this Guid leadId) => new(leadId);

    public static ApplicationConvertirLeadCommand ToConvertirCommand(this Guid leadId) => new(leadId);

    public static ApplicationDescartarLeadCommand ToDescartarCommand(this Guid leadId, DescartarLeadRequest request) => new(leadId, request.Motivo);

    public static LeadEstadoResponse ToContract(this ApplicationLeadEstadoResult result) => new(result.Id, result.Estado);
}
