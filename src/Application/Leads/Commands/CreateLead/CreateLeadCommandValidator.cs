using FluentValidation;
using Plataforma.Domain.Leads;

namespace Plataforma.Application.Leads.Commands.CreateLead;

public sealed class CreateLeadCommandValidator : AbstractValidator<CreateLeadCommand>
{
    public CreateLeadCommandValidator()
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

        RuleFor(x => x.Origen)
            .IsInEnum();

        RuleFor(x => x.PropiedadDeInteresId)
            .NotEqual(Guid.Empty)
            .When(x => x.PropiedadDeInteresId.HasValue);

        RuleFor(x => x.DatosCalculoObra)
            .NotNull()
            .WithMessage("DatosCalculoObra es obligatorio cuando Origen es CalculadoraObra.")
            .When(x => x.Origen == OrigenLead.CalculadoraObra);

        RuleFor(x => x.DatosCalculoObra!.AreaConstruccionM2)
            .GreaterThan(0)
            .LessThanOrEqualTo(100_000)
            .When(x => x.DatosCalculoObra is not null);

        RuleFor(x => x.DatosCalculoObra!.Municipio)
            .NotEmpty()
            .MaximumLength(100)
            .When(x => x.DatosCalculoObra is not null);

        RuleFor(x => x.ServicioDeInteres)
            .IsInEnum()
            .When(x => x.ServicioDeInteres.HasValue);

        RuleFor(x => x.Mensaje)
            .MaximumLength(1000);
    }
}
