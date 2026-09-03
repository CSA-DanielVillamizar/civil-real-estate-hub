using MediatR;
using Plataforma.Application.Tarifas.Commands.Common;

namespace Plataforma.Application.Tarifas.Commands.PublicarPaqueteTarifa;

public sealed record PublicarPaqueteTarifaCommand(Guid PaqueteId) : IRequest<PaqueteTarifaResult?>;
