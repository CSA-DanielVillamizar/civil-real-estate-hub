using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Common.Models;

namespace Plataforma.Application.Properties.Queries.GetPropertiesAdmin;

public sealed class GetPropertiesAdminQueryHandler : IRequestHandler<GetPropertiesAdminQuery, PagedResult<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertiesAdminQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PagedResult<PropertyDto>> Handle(GetPropertiesAdminQuery request, CancellationToken cancellationToken)
    {
        var filter = new PropertyFilter(
            TipoInmueble: null,
            Municipio: null,
            PrecioMin: null,
            PrecioMax: null,
            AreaMin: null,
            AreaMax: null,
            SoloViablesConstructivamente: null,
            Estado: request.Estado);

        var (items, totalCount) = await _propertyRepository.SearchAsync(filter, request.Page, request.PageSize, cancellationToken);
        var dtos = items.Select(PropertyDto.DesdeDominio).ToList();

        return new PagedResult<PropertyDto>(dtos, request.Page, request.PageSize, totalCount);
    }
}
