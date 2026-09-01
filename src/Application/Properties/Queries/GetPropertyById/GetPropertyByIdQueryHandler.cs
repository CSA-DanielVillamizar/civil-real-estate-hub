using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Application.Properties.Queries.GetPropertyById;

public sealed class GetPropertyByIdQueryHandler : IRequestHandler<GetPropertyByIdQuery, PropertyDetailDto?>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertyByIdQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PropertyDetailDto?> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var propiedad = await _propertyRepository.GetByIdAsync(new PropiedadId(request.Id), cancellationToken);
        return propiedad is null ? null : PropertyDetailDto.DesdeDominio(propiedad);
    }
}
