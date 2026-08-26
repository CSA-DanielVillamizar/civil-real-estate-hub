using FluentValidation;
using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Leads;

public sealed class CreateLeadRequestValidator : AbstractValidator<CreateLeadRequest>
{
    public CreateLeadRequestValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(254);

        RuleFor(x => x.Telefono)
            .NotEmpty()
            .Matches(@"^[0-9]{7,15}$")
            .WithMessage("Telefono debe contener solo dígitos (7 a 15).");

        RuleFor(x => x.Indicativo)
            .Matches(@"^\+[0-9]{1,4}$")
            .When(x => x.Indicativo is not null)
            .WithMessage("Indicativo debe tener el formato +57.");

        RuleFor(x => x.Origen)
            .IsInEnum();

        RuleFor(x => x.PropiedadDeInteresId)
            .NotEqual(Guid.Empty)
            .When(x => x.PropiedadDeInteresId.HasValue);

        RuleFor(x => x.DatosCalculoObra)
            .NotNull()
            .WithMessage("DatosCalculoObra es obligatorio cuando Origen es CalculadoraObra.")
            .When(x => x.Origen == OrigenLeadDto.CalculadoraObra);

        RuleFor(x => x.DatosCalculoObra!)
            .SetValidator(new DatosCalculoObraDtoValidator())
            .When(x => x.DatosCalculoObra is not null);
    }
}
