using MediatR;
using Plataforma.Application.Common.Interfaces;

namespace Plataforma.Application.Leads.Queries.ObtenerLeads;

public sealed class ObtenerLeadsQueryHandler : IRequestHandler<ObtenerLeadsQuery, IReadOnlyList<LeadListItem>>
{
    private readonly ILeadRepository _leadRepository;

    public ObtenerLeadsQueryHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<IReadOnlyList<LeadListItem>> Handle(ObtenerLeadsQuery request, CancellationToken cancellationToken)
    {
        var leads = await _leadRepository.ListAsync(request.Estado, cancellationToken);

        return leads
            .Select(l => new LeadListItem(
                l.Id.Value,
                l.Nombre,
                l.Email.Valor,
                l.Telefono.ToString(),
                l.Origen,
                l.Estado,
                l.CapturadoEn,
                l.PropiedadDeInteresId?.Value,
                l.ResultadoCalculadora?.MontoMinimo.Monto,
                l.ResultadoCalculadora?.MontoMaximo.Monto,
                l.ResultadoCalculadora?.MontoMinimo.Moneda))
            .ToList();
    }
}
