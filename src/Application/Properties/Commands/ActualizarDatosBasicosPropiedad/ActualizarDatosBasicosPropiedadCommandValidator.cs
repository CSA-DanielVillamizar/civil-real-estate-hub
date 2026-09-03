using FluentValidation;

namespace Plataforma.Application.Properties.Commands.ActualizarDatosBasicosPropiedad;

public sealed class ActualizarDatosBasicosPropiedadCommandValidator : AbstractValidator<ActualizarDatosBasicosPropiedadCommand>
{
    public ActualizarDatosBasicosPropiedadCommandValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Precio).GreaterThan(0);
        RuleFor(x => x.Moneda).NotEmpty().MaximumLength(3);
    }
}
