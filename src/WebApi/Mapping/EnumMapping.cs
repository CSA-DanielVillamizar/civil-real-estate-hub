using ContractsEnums = Plataforma.Contracts.Common;
using DomainConfianza = Plataforma.Domain.Confianza;
using DomainLeads = Plataforma.Domain.Leads;
using DomainObras = Plataforma.Domain.Obras;
using DomainPropiedades = Plataforma.Domain.Propiedades;
using DomainUsuarios = Plataforma.Domain.Usuarios;
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

    public static DomainPropiedades.EstadoPropiedad ToDomain(this ContractsEnums.EstadoPropiedadDto estado) =>
        Enum.Parse<DomainPropiedades.EstadoPropiedad>(estado.ToString());

    public static DomainViabilidadAmbiental.EstadoSolicitudViabilidad ToDomain(this ContractsEnums.EstadoSolicitudViabilidadDto estado) =>
        Enum.Parse<DomainViabilidadAmbiental.EstadoSolicitudViabilidad>(estado.ToString());

    public static DomainPropiedades.TipoSuelo ToDomain(this ContractsEnums.TipoSueloDto tipoSuelo) =>
        Enum.Parse<DomainPropiedades.TipoSuelo>(tipoSuelo.ToString());

    public static ContractsEnums.TipoSueloDto ToContract(this DomainPropiedades.TipoSuelo tipoSuelo) =>
        Enum.Parse<ContractsEnums.TipoSueloDto>(tipoSuelo.ToString());

    public static DomainPropiedades.Topografia ToDomain(this ContractsEnums.TopografiaDto topografia) =>
        Enum.Parse<DomainPropiedades.Topografia>(topografia.ToString());

    public static ContractsEnums.TopografiaDto ToContract(this DomainPropiedades.Topografia topografia) =>
        Enum.Parse<ContractsEnums.TopografiaDto>(topografia.ToString());

    public static DomainPropiedades.TipoFuenteRetiro ToDomain(this ContractsEnums.TipoFuenteRetiroDto tipoFuente) =>
        Enum.Parse<DomainPropiedades.TipoFuenteRetiro>(tipoFuente.ToString());

    public static ContractsEnums.TipoFuenteRetiroDto ToContract(this DomainPropiedades.TipoFuenteRetiro tipoFuente) =>
        Enum.Parse<ContractsEnums.TipoFuenteRetiroDto>(tipoFuente.ToString());

    public static DomainPropiedades.TipoMultimedia ToDomain(this ContractsEnums.TipoMultimediaDto tipo) =>
        Enum.Parse<DomainPropiedades.TipoMultimedia>(tipo.ToString());

    public static ContractsEnums.TipoMultimediaDto ToContract(this DomainPropiedades.TipoMultimedia tipo) =>
        Enum.Parse<ContractsEnums.TipoMultimediaDto>(tipo.ToString());

    public static DomainPropiedades.UnidadMedidaArea ToDomain(this ContractsEnums.UnidadMedidaAreaDto unidad) =>
        Enum.Parse<DomainPropiedades.UnidadMedidaArea>(unidad.ToString());

    public static DomainLeads.EstadoLead ToDomain(this ContractsEnums.EstadoLeadDto estado) =>
        Enum.Parse<DomainLeads.EstadoLead>(estado.ToString());

    public static ContractsEnums.EstadoLeadDto ToContract(this DomainLeads.EstadoLead estado) =>
        Enum.Parse<ContractsEnums.EstadoLeadDto>(estado.ToString());

    public static ContractsEnums.OrigenLeadDto ToContract(this DomainLeads.OrigenLead origen) =>
        Enum.Parse<ContractsEnums.OrigenLeadDto>(origen.ToString());

    public static DomainLeads.ServicioDeInteres ToDomain(this ContractsEnums.ServicioDeInteresDto servicio) =>
        Enum.Parse<DomainLeads.ServicioDeInteres>(servicio.ToString());

    public static ContractsEnums.ServicioDeInteresDto ToContract(this DomainLeads.ServicioDeInteres servicio) =>
        Enum.Parse<ContractsEnums.ServicioDeInteresDto>(servicio.ToString());

    public static DomainObras.EstadoProyecto ToDomain(this ContractsEnums.EstadoProyectoDto estado) =>
        Enum.Parse<DomainObras.EstadoProyecto>(estado.ToString());

    public static ContractsEnums.EstadoProyectoDto ToContract(this DomainObras.EstadoProyecto estado) =>
        Enum.Parse<ContractsEnums.EstadoProyectoDto>(estado.ToString());

    public static DomainObras.EstadoHito ToDomain(this ContractsEnums.EstadoHitoDto estado) =>
        Enum.Parse<DomainObras.EstadoHito>(estado.ToString());

    public static ContractsEnums.EstadoHitoDto ToContract(this DomainObras.EstadoHito estado) =>
        Enum.Parse<ContractsEnums.EstadoHitoDto>(estado.ToString());

    public static DomainUsuarios.RolUsuario ToDomain(this ContractsEnums.RolUsuarioDto rol) =>
        Enum.Parse<DomainUsuarios.RolUsuario>(rol.ToString());

    public static DomainConfianza.TipoContenidoConfianza ToDomain(this ContractsEnums.TipoContenidoConfianzaDto tipo) =>
        Enum.Parse<DomainConfianza.TipoContenidoConfianza>(tipo.ToString());
}
