using Microsoft.AspNetCore.Identity;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Usuarios;

namespace Plataforma.Infrastructure.Auth;

// Envuelve PasswordHasher<T> de Microsoft.Extensions.Identity.Core (PBKDF2,
// no la librería completa de ASP.NET Core Identity — solo se usa el
// componente de hashing). El dominio nunca ve la contraseña en texto plano
// ni conoce el algoritmo; esta clase es la única que lo hace.
public sealed class PasswordHasherAdapter : IPasswordHasher
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public string Hash(string passwordEnTextoPlano) =>
        _passwordHasher.HashPassword(null!, passwordEnTextoPlano);

    public bool Verificar(string hashAlmacenado, string passwordEnTextoPlano, out bool requiereRehash)
    {
        var resultado = _passwordHasher.VerifyHashedPassword(null!, hashAlmacenado, passwordEnTextoPlano);

        requiereRehash = resultado == PasswordVerificationResult.SuccessRehashNeeded;
        return resultado is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
