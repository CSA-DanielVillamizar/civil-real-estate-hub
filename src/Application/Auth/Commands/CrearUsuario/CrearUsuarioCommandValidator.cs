using FluentValidation;

namespace Plataforma.Application.Auth.Commands.CrearUsuario;

public sealed class CrearUsuarioCommandValidator : AbstractValidator<CrearUsuarioCommand>
{
    public CrearUsuarioCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        // Mínimo razonable para una cuenta con acceso administrativo — no
        // es una app de consumo masivo con miles de usuarios eligiendo su
        // propia contraseña, es el propio Admin sembrando cuentas de equipo.
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Rol).IsInEnum();
    }
}
