using FluentValidation;

namespace Plataforma.Contracts.Obras;

public sealed class CrearProyectoObraRequestValidator : AbstractValidator<CrearProyectoObraRequest>
{
    public CrearProyectoObraRequestValidator()
    {
        RuleFor(x => x.NombreCliente).NotEmpty().MaximumLength(150);
        RuleFor(x => x.EmailCliente).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.TelefonoCliente).NotEmpty().Matches(@"^[0-9]{7,15}$");
        RuleFor(x => x.IndicativoCliente).Matches(@"^\+[0-9]{1,4}$").When(x => x.IndicativoCliente is not null);
        RuleFor(x => x.NombreProyecto).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).MaximumLength(2000);
        RuleFor(x => x.PropiedadId).NotEqual(Guid.Empty).When(x => x.PropiedadId.HasValue);
    }
}
