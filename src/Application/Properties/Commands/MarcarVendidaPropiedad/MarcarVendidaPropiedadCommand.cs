using MediatR;
using Plataforma.Application.Properties.Commands.Common;

namespace Plataforma.Application.Properties.Commands.MarcarVendidaPropiedad;

public sealed record MarcarVendidaPropiedadCommand(Guid PropiedadId) : IRequest<PropertyEstadoResult?>;
