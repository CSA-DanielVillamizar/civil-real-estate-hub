using FluentValidation;

namespace Plataforma.Application.Properties.Queries.GetProperties;

public sealed class GetPropertiesQueryValidator : AbstractValidator<GetPropertiesQuery>
{
    public GetPropertiesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.PrecioMax)
            .GreaterThanOrEqualTo(x => x.PrecioMin)
            .When(x => x.PrecioMin.HasValue && x.PrecioMax.HasValue)
            .WithMessage("PrecioMax debe ser mayor o igual a PrecioMin.");

        RuleFor(x => x.AreaMax)
            .GreaterThanOrEqualTo(x => x.AreaMin)
            .When(x => x.AreaMin.HasValue && x.AreaMax.HasValue)
            .WithMessage("AreaMax debe ser mayor o igual a AreaMin.");
    }
}
