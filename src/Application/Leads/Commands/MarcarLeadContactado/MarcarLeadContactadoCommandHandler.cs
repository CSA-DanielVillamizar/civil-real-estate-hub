using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Leads.Commands.Common;
using Plataforma.Domain.Leads;

namespace Plataforma.Application.Leads.Commands.MarcarLeadContactado;

public sealed class MarcarLeadContactadoCommandHandler : IRequestHandler<MarcarLeadContactadoCommand, LeadEstadoResult?>
{
    private readonly ILeadRepository _leadRepository;

    public MarcarLeadContactadoCommandHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<LeadEstadoResult?> Handle(MarcarLeadContactadoCommand request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(new LeadId(request.LeadId), cancellationToken);
        if (lead is null)
            return null;

        lead.MarcarContactado();
        await _leadRepository.UpdateAsync(lead, cancellationToken);

        return new LeadEstadoResult(lead.Id.Value, lead.Estado.ToString());
    }
}
