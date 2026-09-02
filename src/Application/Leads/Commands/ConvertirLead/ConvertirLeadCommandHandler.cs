using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Leads.Commands.Common;
using Plataforma.Domain.Leads;

namespace Plataforma.Application.Leads.Commands.ConvertirLead;

public sealed class ConvertirLeadCommandHandler : IRequestHandler<ConvertirLeadCommand, LeadEstadoResult?>
{
    private readonly ILeadRepository _leadRepository;

    public ConvertirLeadCommandHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<LeadEstadoResult?> Handle(ConvertirLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(new LeadId(request.LeadId), cancellationToken);
        if (lead is null)
            return null;

        lead.Convertir();
        await _leadRepository.UpdateAsync(lead, cancellationToken);

        return new LeadEstadoResult(lead.Id.Value, lead.Estado.ToString());
    }
}
