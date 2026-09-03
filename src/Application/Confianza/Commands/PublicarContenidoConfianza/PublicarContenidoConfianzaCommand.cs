using MediatR;
using Plataforma.Application.Confianza.Commands.Common;

namespace Plataforma.Application.Confianza.Commands.PublicarContenidoConfianza;

public sealed record PublicarContenidoConfianzaCommand(Guid ContenidoId) : IRequest<ContenidoConfianzaResult?>;
