using FluentValidation;

namespace Plataforma.Contracts.Obras;

public sealed class AgregarHitoRequestValidator : AbstractValidator<AgregarHitoRequest>
{
    public AgregarHitoRequestValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).MaximumLength(2000);
    }
}
