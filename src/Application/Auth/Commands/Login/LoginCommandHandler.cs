using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads.ValueObjects;

namespace Plataforma.Application.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult?>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResult?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Email email;
        try
        {
            email = Email.Crear(request.Email);
        }
        catch (ArgumentException)
        {
            return null;
        }

        var usuario = await _usuarioRepository.GetByEmailAsync(email, cancellationToken);
        if (usuario is null || !usuario.Activo)
            return null;

        var esValida = _passwordHasher.Verificar(usuario.PasswordHash, request.Password, out var requiereRehash);
        if (!esValida)
            return null;

        if (requiereRehash)
        {
            usuario.ActualizarPasswordHash(_passwordHasher.Hash(request.Password));
            await _usuarioRepository.UpdateAsync(usuario, cancellationToken);
        }

        var token = _jwtTokenGenerator.Generar(usuario);
        return new LoginResult(token.Token, token.ExpiraEn, usuario.Nombre, usuario.Rol.ToString());
    }
}
