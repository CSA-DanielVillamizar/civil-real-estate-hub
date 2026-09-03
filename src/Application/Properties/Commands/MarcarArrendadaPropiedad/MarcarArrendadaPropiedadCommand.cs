using MediatR;
using Plataforma.Application.Properties.Commands.Common;

namespace Plataforma.Application.Properties.Commands.MarcarArrendadaPropiedad;

public sealed record MarcarArrendadaPropiedadCommand(Guid PropiedadId) : IRequest<PropertyEstadoResult?>;
