using FluentValidation;

namespace Plataforma.Application.Obras.Commands.AgregarHito;

public sealed class AgregarHitoCommandValidator : AbstractValidator<AgregarHitoCommand>
{
    public AgregarHitoCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).MaximumLength(2000);
    }
}
