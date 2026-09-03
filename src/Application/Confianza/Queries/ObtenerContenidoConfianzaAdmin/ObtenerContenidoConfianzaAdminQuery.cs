using MediatR;
using Plataforma.Application.Confianza.Commands.Common;

namespace Plataforma.Application.Confianza.Queries.ObtenerContenidoConfianzaAdmin;

// Listado administrativo: incluye borradores sin publicar (a diferencia de
// ObtenerContenidoConfianzaPublicadoQuery, que es lo que ve el sitio público).
public sealed record ObtenerContenidoConfianzaAdminQuery : IRequest<IReadOnlyList<ContenidoConfianzaResult>>;
