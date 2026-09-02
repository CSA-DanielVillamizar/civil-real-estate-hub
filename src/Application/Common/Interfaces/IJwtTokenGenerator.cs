using Plataforma.Domain.Usuarios;

namespace Plataforma.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    JwtTokenResult Generar(Usuario usuario);
}

public sealed record JwtTokenResult(string Token, DateTimeOffset ExpiraEn);
