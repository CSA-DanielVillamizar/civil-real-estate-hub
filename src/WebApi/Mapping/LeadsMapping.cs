using Plataforma.Contracts.Leads;
using ApplicationCreateLeadCommand = Plataforma.Application.Leads.Commands.CreateLead.CreateLeadCommand;
using ApplicationCreateLeadResult = Plataforma.Application.Leads.Commands.CreateLead.CreateLeadResult;
using ApplicationDatosCalculoObraInput = Plataforma.Application.Leads.Commands.CreateLead.DatosCalculoObraInput;

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
        request.DatosCalculoObra is null
            ? null
            : new ApplicationDatosCalculoObraInput(
                request.DatosCalculoObra.AreaConstruccionM2,
                request.DatosCalculoObra.TipoAcabado.ToDomain(),
                request.DatosCalculoObra.Municipio,
                request.DatosCalculoObra.TipoProyecto.ToDomain()));

    public static CreateLeadResponse ToContract(this ApplicationCreateLeadResult result) => new(
        result.Id,
        result.Estado,
        result.EstimacionCosto?.ToContract());
}
