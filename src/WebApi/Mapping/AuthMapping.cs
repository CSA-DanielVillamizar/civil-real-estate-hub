using Plataforma.Application.Auth.Commands.Login;
using Plataforma.Contracts.Auth;

namespace Plataforma.WebApi.Mapping;

public static class AuthMapping
{
    public static LoginCommand ToCommand(this LoginRequest request) =>
        new(request.Email, request.Password);

    public static LoginResponse ToContract(this LoginResult result) =>
        new(result.Token, result.ExpiraEn, result.Nombre, result.Rol);
}
