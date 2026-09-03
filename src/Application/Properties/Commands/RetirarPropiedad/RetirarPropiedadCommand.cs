using MediatR;
using Plataforma.Application.Properties.Commands.Common;

namespace Plataforma.Application.Properties.Commands.RetirarPropiedad;

public sealed record RetirarPropiedadCommand(Guid PropiedadId) : IRequest<PropertyEstadoResult?>;
