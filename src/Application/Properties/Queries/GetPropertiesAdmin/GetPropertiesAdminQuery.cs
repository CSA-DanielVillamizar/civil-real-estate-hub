using MediatR;
using Plataforma.Application.Common.Models;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Application.Properties.Queries.GetPropertiesAdmin;

// A diferencia de GetPropertiesQuery (público, siempre fuerza Estado =
// Publicada — ver el comentario en su handler), esta query es
// administrativa: Estado es un filtro real que el llamador controla
// (null = todas, cualquier estado, incluyendo Borrador) — necesaria para
// que el panel admin encuentre los borradores pendientes de publicar.
public sealed record GetPropertiesAdminQuery(
    EstadoPropiedad? Estado,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<PropertyDto>>;
