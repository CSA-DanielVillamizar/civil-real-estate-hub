using MediatR;
using Plataforma.Application.Properties.Commands.Common;

namespace Plataforma.Application.Properties.Commands.ReservarPropiedad;

public sealed record ReservarPropiedadCommand(Guid PropiedadId) : IRequest<PropertyEstadoResult?>;
