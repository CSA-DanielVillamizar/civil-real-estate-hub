using FluentValidation;

namespace Plataforma.Application.Properties.Queries.GetPropertiesAdmin;

public sealed class GetPropertiesAdminQueryValidator : AbstractValidator<GetPropertiesAdminQuery>
{
    public GetPropertiesAdminQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
