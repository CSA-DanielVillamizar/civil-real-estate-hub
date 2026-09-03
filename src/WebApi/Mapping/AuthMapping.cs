using Plataforma.Application.Auth.Commands.CambiarActivoUsuario;
using Plataforma.Application.Auth.Commands.CrearUsuario;
using Plataforma.Application.Auth.Commands.Login;
using Plataforma.Application.Auth.Queries.ObtenerUsuarios;
using Plataforma.Contracts.Auth;

namespace Plataforma.WebApi.Mapping;

public static class AuthMapping
{
    public static LoginCommand ToCommand(this LoginRequest request) =>
        new(request.Email, request.Password);

    public static LoginResponse ToContract(this LoginResult result) =>
        new(result.Token, result.ExpiraEn, result.Nombre, result.Rol);

    public static CrearUsuarioCommand ToCommand(this CrearUsuarioRequest request) =>
        new(request.Nombre, request.Email, request.Password, request.Rol.ToDomain());

    public static CrearUsuarioResponse ToContract(this CrearUsuarioResult result) =>
        new(result.Id, result.Nombre, result.Email, result.Rol);

    public static UsuarioListItemDto ToContract(this UsuarioListItem item) =>
        new(item.Id, item.Nombre, item.Email, item.Rol, item.Activo, item.CreadoEn);

    public static CambiarActivoUsuarioCommand ToCommand(this CambiarActivoUsuarioRequest request, Guid usuarioId) =>
        new(usuarioId, request.Activo);

    public static CambiarActivoUsuarioResponse ToContract(this CambiarActivoUsuarioResult result) =>
        new(result.Id, result.Activo);
}
