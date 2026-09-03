using MediatR;
using Plataforma.Application.Confianza.Commands.Common;

namespace Plataforma.Application.Confianza.Queries.ObtenerContenidoConfianzaPublicado;

public sealed record ObtenerContenidoConfianzaPublicadoQuery : IRequest<IReadOnlyList<ContenidoConfianzaResult>>;
