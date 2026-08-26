using Plataforma.Application.Common.Models;
using Plataforma.Application.Properties;
using Plataforma.Contracts.Common;
using Plataforma.Contracts.Properties;
using ApplicationGetPropertiesQuery = Plataforma.Application.Properties.Queries.GetProperties.GetPropertiesQuery;
using ContractsGetPropertiesQuery = Plataforma.Contracts.Properties.GetPropertiesQuery;

namespace Plataforma.WebApi.Mapping;

public static class PropertiesMapping
{
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
