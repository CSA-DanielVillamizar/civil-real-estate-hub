using FluentValidation;

namespace Plataforma.Contracts.Confianza;

public sealed class CrearContenidoConfianzaRequestValidator : AbstractValidator<CrearContenidoConfianzaRequest>
{
    public CrearContenidoConfianzaRequestValidator()
    {
        RuleFor(x => x.Tipo).IsInEnum();
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Municipio).MaximumLength(100);
        RuleFor(x => x.ServicioRelacionado).IsInEnum();
    }
}
