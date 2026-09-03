using FluentValidation;

namespace Plataforma.Contracts.Properties;

public sealed class ActualizarDatosBasicosPropiedadRequestValidator : AbstractValidator<ActualizarDatosBasicosPropiedadRequest>
{
    public ActualizarDatosBasicosPropiedadRequestValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Precio).GreaterThan(0);
        RuleFor(x => x.Moneda).NotEmpty().MaximumLength(3);
    }
}
