namespace Plataforma.Contracts.Auth;

public sealed record LoginResponse(string Token, DateTimeOffset ExpiraEn, string Nombre, string Rol);
