using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Leads.Commands.Common;
using Plataforma.Domain.Leads;

namespace Plataforma.Application.Leads.Commands.CalificarLead;

public sealed class CalificarLeadCommandHandler : IRequestHandler<CalificarLeadCommand, LeadEstadoResult?>
{
    private readonly ILeadRepository _leadRepository;

    public CalificarLeadCommandHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<LeadEstadoResult?> Handle(CalificarLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(new LeadId(request.LeadId), cancellationToken);
        if (lead is null)
            return null;

        lead.Calificar();
        await _leadRepository.UpdateAsync(lead, cancellationToken);

        return new LeadEstadoResult(lead.Id.Value, lead.Estado.ToString());
    }
}
