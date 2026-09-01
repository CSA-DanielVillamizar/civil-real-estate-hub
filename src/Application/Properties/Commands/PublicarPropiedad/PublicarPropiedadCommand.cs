using MediatR;

namespace Plataforma.Application.Properties.Commands.PublicarPropiedad;

public sealed record PublicarPropiedadCommand(Guid PropiedadId) : IRequest<PublicarPropiedadResult?>;
