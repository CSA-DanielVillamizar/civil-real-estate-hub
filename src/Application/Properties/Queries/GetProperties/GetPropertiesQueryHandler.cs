using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Common.Models;

namespace Plataforma.Application.Properties.Queries.GetProperties;

public sealed class GetPropertiesQueryHandler : IRequestHandler<GetPropertiesQuery, PagedResult<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertiesQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PagedResult<PropertyDto>> Handle(GetPropertiesQuery request, CancellationToken cancellationToken)
    {
        var filter = new PropertyFilter(
            request.TipoInmueble,
            request.Municipio,
            request.PrecioMin,
            request.PrecioMax,
            request.AreaMin,
            request.AreaMax,
            request.SoloViablesConstructivamente);

        var (items, totalCount) = await _propertyRepository.SearchAsync(
            filter,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(PropertyDto.DesdeDominio).ToList();

        return new PagedResult<PropertyDto>(dtos, request.Page, request.PageSize, totalCount);
    }
}
