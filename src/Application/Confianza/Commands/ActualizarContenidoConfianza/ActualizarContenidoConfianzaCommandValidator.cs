using FluentValidation;

namespace Plataforma.Application.Confianza.Commands.ActualizarContenidoConfianza;

public sealed class ActualizarContenidoConfianzaCommandValidator : AbstractValidator<ActualizarContenidoConfianzaCommand>
{
    public ActualizarContenidoConfianzaCommandValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Municipio).MaximumLength(100);
        RuleFor(x => x.ServicioRelacionado).IsInEnum();
    }
}
