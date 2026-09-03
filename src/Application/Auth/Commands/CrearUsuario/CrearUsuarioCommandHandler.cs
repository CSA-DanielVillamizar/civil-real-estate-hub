using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Usuarios;
using Plataforma.Domain.Usuarios.Exceptions;

namespace Plataforma.Application.Auth.Commands.CrearUsuario;

public sealed class CrearUsuarioCommandHandler : IRequestHandler<CrearUsuarioCommand, CrearUsuarioResult>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CrearUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<CrearUsuarioResult> Handle(CrearUsuarioCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Crear(request.Email);

        var existente = await _usuarioRepository.GetByEmailAsync(email, cancellationToken);
        if (existente is not null)
            throw new EmailYaRegistradoException($"Ya existe un usuario con el email {request.Email}.");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var usuario = Usuario.Crear(request.Nombre, email, passwordHash, request.Rol);

        await _usuarioRepository.AddAsync(usuario, cancellationToken);

        return new CrearUsuarioResult(usuario.Id.Value, usuario.Nombre, usuario.Email.Valor, usuario.Rol.ToString());
    }
}
