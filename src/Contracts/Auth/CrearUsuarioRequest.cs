using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Auth;

public sealed record CrearUsuarioRequest(string Nombre, string Email, string Password, RolUsuarioDto Rol);
