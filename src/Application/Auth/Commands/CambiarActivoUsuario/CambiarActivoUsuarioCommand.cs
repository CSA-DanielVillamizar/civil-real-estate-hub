using MediatR;

namespace Plataforma.Application.Auth.Commands.CambiarActivoUsuario;

public sealed record CambiarActivoUsuarioCommand(Guid UsuarioId, bool Activo) : IRequest<CambiarActivoUsuarioResult?>;

public sealed record CambiarActivoUsuarioResult(Guid Id, bool Activo);
