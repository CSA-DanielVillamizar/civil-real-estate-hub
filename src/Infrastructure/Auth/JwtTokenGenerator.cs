using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Usuarios;

namespace Plataforma.Infrastructure.Auth;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public JwtTokenResult Generar(Usuario usuario)
    {
        var expiraEn = DateTimeOffset.UtcNow.AddMinutes(_options.ExpiracionEnMinutos);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.Value.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email.Valor),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
        };

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiraEn.UtcDateTime,
            signingCredentials: credenciales);

        return new JwtTokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiraEn);
    }
}
