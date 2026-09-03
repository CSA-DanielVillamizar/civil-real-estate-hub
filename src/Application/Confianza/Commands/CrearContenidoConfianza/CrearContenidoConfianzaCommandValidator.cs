using FluentValidation;

namespace Plataforma.Application.Confianza.Commands.CrearContenidoConfianza;

public sealed class CrearContenidoConfianzaCommandValidator : AbstractValidator<CrearContenidoConfianzaCommand>
{
    public CrearContenidoConfianzaCommandValidator()
    {
        RuleFor(x => x.Tipo).IsInEnum();
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Municipio).MaximumLength(100);
        RuleFor(x => x.ServicioRelacionado).IsInEnum();
    }
}
