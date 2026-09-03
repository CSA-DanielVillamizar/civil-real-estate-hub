using FluentValidation;

namespace Plataforma.Contracts.Tarifas;

public sealed class ActualizarPaqueteTarifaRequestValidator : AbstractValidator<ActualizarPaqueteTarifaRequest>
{
    public ActualizarPaqueteTarifaRequestValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.PrecioDesde).GreaterThanOrEqualTo(0).When(x => x.PrecioDesde.HasValue);
        RuleFor(x => x.PrecioHasta).GreaterThanOrEqualTo(0).When(x => x.PrecioHasta.HasValue);
        RuleFor(x => x.PrecioDesde)
            .LessThanOrEqualTo(x => x.PrecioHasta!.Value)
            .When(x => x.PrecioDesde.HasValue && x.PrecioHasta.HasValue)
            .WithMessage("El precio desde no puede ser mayor que el precio hasta.");
        RuleFor(x => x.UnidadPrecio).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ServicioRelacionado).IsInEnum();
    }
}
