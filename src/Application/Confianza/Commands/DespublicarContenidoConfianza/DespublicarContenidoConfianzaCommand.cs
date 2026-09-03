using MediatR;
using Plataforma.Application.Confianza.Commands.Common;

namespace Plataforma.Application.Confianza.Commands.DespublicarContenidoConfianza;

public sealed record DespublicarContenidoConfianzaCommand(Guid ContenidoId) : IRequest<ContenidoConfianzaResult?>;
