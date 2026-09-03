using MediatR;
using Plataforma.Application.Tarifas.Commands.Common;

namespace Plataforma.Application.Tarifas.Queries.ObtenerPaquetesTarifaAdmin;

public sealed record ObtenerPaquetesTarifaAdminQuery : IRequest<IReadOnlyList<PaqueteTarifaResult>>;
