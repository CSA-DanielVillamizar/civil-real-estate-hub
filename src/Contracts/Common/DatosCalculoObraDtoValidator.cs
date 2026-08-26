using FluentValidation;

namespace Plataforma.Contracts.Common;

public sealed class DatosCalculoObraDtoValidator : AbstractValidator<DatosCalculoObraDto>
{
    public DatosCalculoObraDtoValidator()
    {
        RuleFor(x => x.AreaConstruccionM2)
            .GreaterThan(0)
            .LessThanOrEqualTo(100_000);

        RuleFor(x => x.TipoAcabado)
            .IsInEnum();

        RuleFor(x => x.Municipio)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.TipoProyecto)
            .IsInEnum();
    }
}
