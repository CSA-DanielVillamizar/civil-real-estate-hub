using Plataforma.Contracts.Leads;
using ApplicationCreateLeadCommand = Plataforma.Application.Leads.Commands.CreateLead.CreateLeadCommand;
using ApplicationCreateLeadResult = Plataforma.Application.Leads.Commands.CreateLead.CreateLeadResult;
using ApplicationDatosCalculoObraInput = Plataforma.Application.Leads.Commands.CreateLead.DatosCalculoObraInput;
using ApplicationGenerarPresupuestoPdfCommand = Plataforma.Application.Leads.Commands.GenerarPresupuestoPdf.GenerarPresupuestoPdfCommand;

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
        request.DatosCalculoObra.ToApplicationInput());

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
}
