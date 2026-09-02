using MediatR;

namespace Plataforma.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResult?>;

// Null cuando las credenciales no son válidas (email inexistente, password
// incorrecta o usuario inactivo) — deliberadamente no distingue el motivo en
// la respuesta, para no filtrar qué correos existen en el sistema.
public sealed record LoginResult(string Token, DateTimeOffset ExpiraEn, string Nombre, string Rol);
