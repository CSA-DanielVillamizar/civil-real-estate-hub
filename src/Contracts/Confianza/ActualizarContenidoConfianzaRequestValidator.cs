using FluentValidation;

namespace Plataforma.Contracts.Confianza;

public sealed class ActualizarContenidoConfianzaRequestValidator : AbstractValidator<ActualizarContenidoConfianzaRequest>
{
    public ActualizarContenidoConfianzaRequestValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Municipio).MaximumLength(100);
        RuleFor(x => x.ServicioRelacionado).IsInEnum();
    }
}
