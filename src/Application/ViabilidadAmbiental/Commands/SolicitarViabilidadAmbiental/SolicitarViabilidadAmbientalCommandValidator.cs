using FluentValidation;

namespace Plataforma.Application.ViabilidadAmbiental.Commands.SolicitarViabilidadAmbiental;

public sealed class SolicitarViabilidadAmbientalCommandValidator : AbstractValidator<SolicitarViabilidadAmbientalCommand>
{
    public SolicitarViabilidadAmbientalCommandValidator()
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
            .Matches(@"^[0-9]{7,15}$");

        RuleFor(x => x.Indicativo)
            .Matches(@"^\+[0-9]{1,4}$")
            .When(x => x.Indicativo is not null);

        RuleFor(x => x.PropiedadId)
            .NotEqual(Guid.Empty)
            .When(x => x.PropiedadId.HasValue);

        // Invariante de mutua exclusión (ver también SolicitudViabilidadAmbiental.Solicitar,
        // que la vuelve a exigir a nivel de dominio): exactamente uno de los dos
        // datos de ubicación debe llegar.
        RuleFor(x => x)
            .Must(x => x.PropiedadId.HasValue || !string.IsNullOrWhiteSpace(x.Departamento))
            .WithMessage("Debe indicarse propiedadId o departamento/municipio del lote.")
            .WithName("PropiedadId");

        RuleFor(x => x.Departamento)
            .MaximumLength(100)
            .NotEmpty()
            .When(x => !x.PropiedadId.HasValue);

        RuleFor(x => x.Municipio)
            .MaximumLength(100)
            .NotEmpty()
            .When(x => !x.PropiedadId.HasValue);

        RuleFor(x => x.DireccionReferencia)
            .MaximumLength(250);
    }
}
