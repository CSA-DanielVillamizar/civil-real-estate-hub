using MediatR;

namespace Plataforma.Application.Properties.Queries.GetPropertyById;

public sealed record GetPropertyByIdQuery(Guid Id) : IRequest<PropertyDetailDto?>;
