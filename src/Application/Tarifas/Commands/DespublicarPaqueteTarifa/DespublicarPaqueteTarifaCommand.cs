using MediatR;
using Plataforma.Application.Tarifas.Commands.Common;

namespace Plataforma.Application.Tarifas.Commands.DespublicarPaqueteTarifa;

public sealed record DespublicarPaqueteTarifaCommand(Guid PaqueteId) : IRequest<PaqueteTarifaResult?>;
