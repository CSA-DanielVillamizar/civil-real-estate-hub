using FluentValidation;

namespace Plataforma.Contracts.ViabilidadAmbiental;

public sealed class SolicitarViabilidadAmbientalRequestValidator : AbstractValidator<SolicitarViabilidadAmbientalRequest>
{
    public SolicitarViabilidadAmbientalRequestValidator()
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

        RuleFor(x => x.PropiedadId)
            .NotEqual(Guid.Empty)
            .When(x => x.PropiedadId.HasValue);

        RuleFor(x => x)
            .Must(x => x.PropiedadId.HasValue || !string.IsNullOrWhiteSpace(x.Departamento))
            .WithMessage("Debe indicarse propiedadId o departamento/municipio del lote.")
            .WithName("PropiedadId");

        RuleFor(x => x.Departamento)
            .NotEmpty()
            .MaximumLength(100)
            .When(x => !x.PropiedadId.HasValue);

        RuleFor(x => x.Municipio)
            .NotEmpty()
            .MaximumLength(100)
            .When(x => !x.PropiedadId.HasValue);

        RuleFor(x => x.DireccionReferencia)
            .MaximumLength(250);
    }
}
