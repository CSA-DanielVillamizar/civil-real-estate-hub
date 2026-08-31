using FluentValidation;

namespace Plataforma.Application.Leads.Commands.GenerarPresupuestoPdf;

public sealed class GenerarPresupuestoPdfCommandValidator : AbstractValidator<GenerarPresupuestoPdfCommand>
{
    public GenerarPresupuestoPdfCommandValidator()
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

        RuleFor(x => x.PropiedadDeInteresId)
            .NotEqual(Guid.Empty)
            .When(x => x.PropiedadDeInteresId.HasValue);

        RuleFor(x => x.DatosCalculoObra)
            .NotNull();

        // El operador ! es seguro aquí: .When() evita que FluentValidation
        // evalúe estas reglas cuando DatosCalculoObra es null (ya cubierto
        // por el NotNull() de arriba, que reporta ese caso limpiamente).
        RuleFor(x => x.DatosCalculoObra!.AreaConstruccionM2)
            .GreaterThan(0)
            .LessThanOrEqualTo(100_000)
            .When(x => x.DatosCalculoObra is not null);

        RuleFor(x => x.DatosCalculoObra!.Municipio)
            .NotEmpty()
            .MaximumLength(100)
            .When(x => x.DatosCalculoObra is not null);
    }
}
