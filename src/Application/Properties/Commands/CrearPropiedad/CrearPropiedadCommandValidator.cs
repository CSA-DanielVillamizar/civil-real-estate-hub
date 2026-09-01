using FluentValidation;

namespace Plataforma.Application.Properties.Commands.CrearPropiedad;

public sealed class CrearPropiedadCommandValidator : AbstractValidator<CrearPropiedadCommand>
{
    public CrearPropiedadCommandValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).NotEmpty();
        RuleFor(x => x.TipoInmueble).IsInEnum();

        RuleFor(x => x.Precio).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Moneda).NotEmpty().MaximumLength(3);

        RuleFor(x => x.Direccion).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Municipio).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Departamento).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Latitud).InclusiveBetween(-90, 90).When(x => x.Latitud.HasValue);
        RuleFor(x => x.Longitud).InclusiveBetween(-180, 180).When(x => x.Longitud.HasValue);

        RuleFor(x => x.AreaTerrenoValor).GreaterThan(0);
        RuleFor(x => x.AreaTerrenoUnidad).IsInEnum();

        RuleFor(x => x.AreaConstruidaValor).GreaterThan(0).When(x => x.AreaConstruidaValor.HasValue);

        RuleFor(x => x.PendientePorcentaje).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TipoSuelo).IsInEnum();
        RuleFor(x => x.Topografia).IsInEnum();
        RuleFor(x => x.NivelFreaticoMetros).GreaterThanOrEqualTo(0).When(x => x.NivelFreaticoMetros.HasValue);

        RuleForEach(x => x.RetirosAmbientales).ChildRules(retiro =>
        {
            retiro.RuleFor(r => r.TipoFuente).IsInEnum();
            retiro.RuleFor(r => r.DistanciaMinimaMetros).GreaterThan(0);
            retiro.RuleFor(r => r.NormativaAplicable).NotEmpty().MaximumLength(300);
        });
    }
}
