using MediatR;
using Plataforma.Application.Tarifas.Commands.Common;

namespace Plataforma.Application.Tarifas.Queries.ObtenerPaquetesTarifaPublicados;

public sealed record ObtenerPaquetesTarifaPublicadosQuery : IRequest<IReadOnlyList<PaqueteTarifaResult>>;
