using FluentValidation;

namespace Plataforma.Application.Leads.Commands.DescartarLead;

public sealed class DescartarLeadCommandValidator : AbstractValidator<DescartarLeadCommand>
{
    public DescartarLeadCommandValidator()
    {
        RuleFor(x => x.Motivo).NotEmpty().MaximumLength(500);
    }
}
