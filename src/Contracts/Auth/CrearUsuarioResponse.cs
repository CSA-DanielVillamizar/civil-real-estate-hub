namespace Plataforma.Contracts.Auth;

public sealed record CrearUsuarioResponse(Guid Id, string Nombre, string Email, string Rol);
