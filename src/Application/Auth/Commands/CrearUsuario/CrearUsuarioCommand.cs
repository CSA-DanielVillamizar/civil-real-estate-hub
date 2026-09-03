using MediatR;
using Plataforma.Domain.Usuarios;

namespace Plataforma.Application.Auth.Commands.CrearUsuario;

public sealed record CrearUsuarioCommand(string Nombre, string Email, string Password, RolUsuario Rol) : IRequest<CrearUsuarioResult>;

public sealed record CrearUsuarioResult(Guid Id, string Nombre, string Email, string Rol);
