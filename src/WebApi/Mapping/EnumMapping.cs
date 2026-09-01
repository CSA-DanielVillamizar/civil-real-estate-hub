using ContractsEnums = Plataforma.Contracts.Common;
using DomainLeads = Plataforma.Domain.Leads;
using DomainPropiedades = Plataforma.Domain.Propiedades;
using DomainViabilidadAmbiental = Plataforma.Domain.ViabilidadAmbiental;

namespace Plataforma.WebApi.Mapping;

// Los enums de Contracts (Fase 2) y de Domain (Fase 3) se diseñaron con los
// mismos nombres de miembro a propósito — la conversión por nombre (en vez de
// un cast ordinal) falla rápido y explícito si algún enum diverge en el futuro.
public static class EnumMapping
{
    public static DomainLeads.OrigenLead ToDomain(this ContractsEnums.OrigenLeadDto origen) =>
        Enum.Parse<DomainLeads.OrigenLead>(origen.ToString());

    public static DomainLeads.TipoAcabado ToDomain(this ContractsEnums.TipoAcabadoDto tipoAcabado) =>
        Enum.Parse<DomainLeads.TipoAcabado>(tipoAcabado.ToString());

    public static DomainLeads.TipoProyecto ToDomain(this ContractsEnums.TipoProyectoDto tipoProyecto) =>
        Enum.Parse<DomainLeads.TipoProyecto>(tipoProyecto.ToString());

    public static DomainPropiedades.TipoInmueble ToDomain(this ContractsEnums.TipoInmuebleDto tipoInmueble) =>
        Enum.Parse<DomainPropiedades.TipoInmueble>(tipoInmueble.ToString());

    public static ContractsEnums.TipoInmuebleDto ToContract(this DomainPropiedades.TipoInmueble tipoInmueble) =>
        Enum.Parse<ContractsEnums.TipoInmuebleDto>(tipoInmueble.ToString());

    public static ContractsEnums.EstadoPropiedadDto ToContract(this DomainPropiedades.EstadoPropiedad estado) =>
        Enum.Parse<ContractsEnums.EstadoPropiedadDto>(estado.ToString());

    public static DomainViabilidadAmbiental.EstadoSolicitudViabilidad ToDomain(this ContractsEnums.EstadoSolicitudViabilidadDto estado) =>
        Enum.Parse<DomainViabilidadAmbiental.EstadoSolicitudViabilidad>(estado.ToString());
}
