using Plataforma.Application.Common.Models;
using Plataforma.Application.Properties;
using Plataforma.Contracts.Common;
using Plataforma.Contracts.Properties;
using ApplicationActualizarDatosBasicosCommand = Plataforma.Application.Properties.Commands.ActualizarDatosBasicosPropiedad.ActualizarDatosBasicosPropiedadCommand;
using ApplicationActualizarDatosBasicosResult = Plataforma.Application.Properties.Commands.ActualizarDatosBasicosPropiedad.ActualizarDatosBasicosPropiedadResult;
using ApplicationAgregarMultimediaCommand = Plataforma.Application.Properties.Commands.AgregarMultimediaAPropiedad.AgregarMultimediaAPropiedadCommand;
using ApplicationAgregarMultimediaResult = Plataforma.Application.Properties.Commands.AgregarMultimediaAPropiedad.AgregarMultimediaAPropiedadResult;
using ApplicationCrearPropiedadCommand = Plataforma.Application.Properties.Commands.CrearPropiedad.CrearPropiedadCommand;
using ApplicationCrearPropiedadResult = Plataforma.Application.Properties.Commands.CrearPropiedad.CrearPropiedadResult;
using ApplicationGetPropertiesAdminQuery = Plataforma.Application.Properties.Queries.GetPropertiesAdmin.GetPropertiesAdminQuery;
using ApplicationGetPropertiesQuery = Plataforma.Application.Properties.Queries.GetProperties.GetPropertiesQuery;
using ApplicationGetPropertyByIdQuery = Plataforma.Application.Properties.Queries.GetPropertyById.GetPropertyByIdQuery;
using ApplicationMarcarArrendadaCommand = Plataforma.Application.Properties.Commands.MarcarArrendadaPropiedad.MarcarArrendadaPropiedadCommand;
using ApplicationMarcarVendidaCommand = Plataforma.Application.Properties.Commands.MarcarVendidaPropiedad.MarcarVendidaPropiedadCommand;
using ApplicationPropertyDetailDto = Plataforma.Application.Properties.Queries.GetPropertyById.PropertyDetailDto;
using ApplicationPropertyEstadoResult = Plataforma.Application.Properties.Commands.Common.PropertyEstadoResult;
using ApplicationPublicarPropiedadCommand = Plataforma.Application.Properties.Commands.PublicarPropiedad.PublicarPropiedadCommand;
using ApplicationPublicarPropiedadResult = Plataforma.Application.Properties.Commands.PublicarPropiedad.PublicarPropiedadResult;
using ApplicationReservarCommand = Plataforma.Application.Properties.Commands.ReservarPropiedad.ReservarPropiedadCommand;
using ApplicationRetirarCommand = Plataforma.Application.Properties.Commands.RetirarPropiedad.RetirarPropiedadCommand;
using ApplicationRetiroAmbientalInput = Plataforma.Application.Properties.Commands.CrearPropiedad.RetiroAmbientalInput;
using ContractsGetPropertiesQuery = Plataforma.Contracts.Properties.GetPropertiesQuery;

namespace Plataforma.WebApi.Mapping;

public static class PropertiesMapping
{
    public static ApplicationCrearPropiedadCommand ToCommand(this CrearPropiedadRequest request) => new(
        request.Titulo,
        request.Descripcion,
        request.TipoInmueble.ToDomain(),
        request.Precio,
        request.Moneda,
        request.Direccion,
        request.Municipio,
        request.Departamento,
        request.Latitud,
        request.Longitud,
        request.AreaTerrenoValor,
        request.AreaTerrenoUnidad.ToDomain(),
        request.AreaConstruidaValor,
        request.AreaConstruidaUnidad?.ToDomain(),
        request.PendientePorcentaje,
        request.TipoSuelo.ToDomain(),
        request.Topografia.ToDomain(),
        request.NivelFreaticoMetros,
        request.RetirosAmbientales?
            .Select(r => new ApplicationRetiroAmbientalInput(r.TipoFuente.ToDomain(), r.DistanciaMinimaMetros, r.NormativaAplicable))
            .ToList());

    public static CrearPropiedadResponse ToContract(this ApplicationCrearPropiedadResult result) => new(result.Id, result.Estado);

    public static ApplicationPublicarPropiedadCommand ToPublicarCommand(this Guid propiedadId) => new(propiedadId);

    public static PublicarPropiedadResponse ToContract(this ApplicationPublicarPropiedadResult result) => new(result.Id, result.Estado);

    public static ApplicationReservarCommand ToReservarCommand(this Guid propiedadId) => new(propiedadId);

    public static ApplicationMarcarVendidaCommand ToMarcarVendidaCommand(this Guid propiedadId) => new(propiedadId);

    public static ApplicationMarcarArrendadaCommand ToMarcarArrendadaCommand(this Guid propiedadId) => new(propiedadId);

    public static ApplicationRetirarCommand ToRetirarCommand(this Guid propiedadId) => new(propiedadId);

    // Mismo shape que PublicarPropiedadResponse (Id + Estado) — se reutiliza
    // en vez de crear 4 DTOs casi idénticos para las otras transiciones de
    // estado dentro de este mismo bounded context.
    public static PublicarPropiedadResponse ToContract(this ApplicationPropertyEstadoResult result) => new(result.Id, result.Estado);

    public static ApplicationActualizarDatosBasicosCommand ToCommand(this ActualizarDatosBasicosPropiedadRequest request, Guid propiedadId) =>
        new(propiedadId, request.Titulo, request.Descripcion, request.Precio, request.Moneda);

    public static ActualizarDatosBasicosPropiedadResponse ToContract(this ApplicationActualizarDatosBasicosResult result) => new(
        result.Id, result.Titulo, result.Descripcion, result.Precio, result.Moneda);

    public static ApplicationGetPropertyByIdQuery ToGetByIdQuery(this Guid id) => new(id);

    public static ApplicationGetPropertiesAdminQuery ToApplicationQuery(this GetPropertiesAdminQuery query) => new(
        query.Estado?.ToDomain(), query.Page, query.PageSize);

    public static AgregarMultimediaResponse ToContract(this ApplicationAgregarMultimediaResult result) => new(
        result.PropiedadId, result.Url, result.Tipo);

    public static PropertyDetailResponse ToContract(this ApplicationPropertyDetailDto dto) => new(
        dto.Id,
        dto.Titulo,
        dto.Descripcion,
        dto.TipoInmueble.ToContract(),
        dto.Precio,
        dto.Moneda,
        dto.Direccion,
        dto.Municipio,
        dto.Departamento,
        dto.Latitud,
        dto.Longitud,
        dto.AreaTerrenoM2,
        dto.AreaConstruidaM2,
        dto.PendientePorcentaje,
        dto.TipoSuelo.ToContract(),
        dto.Topografia.ToContract(),
        dto.NivelFreaticoMetros,
        dto.Estado.ToContract(),
        dto.EsViableConstructivamente,
        dto.RestriccionesViabilidad,
        dto.RetirosAmbientales.Select(r => new RetiroAmbientalResponseDto(r.TipoFuente.ToContract(), r.DistanciaMinimaMetros, r.NormativaAplicable)).ToList(),
        dto.Multimedia.Select(m => new ArchivoMultimediaResponseDto(m.Id, m.Url, m.Tipo.ToContract(), m.Orden)).ToList());

    public static ApplicationGetPropertiesQuery ToApplicationQuery(this ContractsGetPropertiesQuery query) => new(
        query.TipoInmueble?.ToDomain(),
        query.Municipio,
        query.PrecioMin,
        query.PrecioMax,
        query.AreaMin,
        query.AreaMax,
        query.SoloViablesConstructivamente,
        query.Page,
        query.PageSize);

    public static PropertyResponse ToContract(this PropertyDto dto) => new(
        dto.Id,
        dto.Titulo,
        dto.TipoInmueble.ToContract(),
        dto.Precio,
        dto.Moneda,
        dto.Municipio,
        dto.Departamento,
        dto.AreaTerrenoM2,
        dto.AreaConstruidaM2,
        dto.Estado.ToContract(),
        dto.FotoPrincipalUrl,
        dto.EsViableConstructivamente);

    public static PagedResponse<PropertyResponse> ToContract(this PagedResult<PropertyDto> result) => new(
        result.Items.Select(item => item.ToContract()).ToList(),
        result.Page,
        result.PageSize,
        result.TotalCount);
}
