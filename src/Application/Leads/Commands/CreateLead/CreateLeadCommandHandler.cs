using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Leads.Services;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Application.Leads.Commands.CreateLead;

public sealed class CreateLeadCommandHandler : IRequestHandler<CreateLeadCommand, CreateLeadResult>
{
    private readonly ILeadRepository _leadRepository;
    private readonly CalculadoraDeObraService _calculadoraDeObraService;

    public CreateLeadCommandHandler(ILeadRepository leadRepository, CalculadoraDeObraService calculadoraDeObraService)
    {
        _leadRepository = leadRepository;
        _calculadoraDeObraService = calculadoraDeObraService;
    }

    public async Task<CreateLeadResult> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Crear(request.Email);
        var telefono = Telefono.Crear(request.Telefono, request.Indicativo);

        EstimacionCosto? estimacionCosto = null;
        if (request.DatosCalculoObra is not null)
        {
            var datosCalculoObra = DatosCalculoObra.Crear(
                request.DatosCalculoObra.AreaConstruccionM2,
                request.DatosCalculoObra.TipoAcabado,
                request.DatosCalculoObra.Municipio,
                request.DatosCalculoObra.TipoProyecto);

            estimacionCosto = _calculadoraDeObraService.Calcular(datosCalculoObra);
        }

        var propiedadDeInteresId = request.PropiedadDeInteresId.HasValue
            ? new PropiedadId(request.PropiedadDeInteresId.Value)
            : (PropiedadId?)null;

        var lead = Lead.Registrar(
            request.Nombre,
            email,
            telefono,
            request.Origen,
            propiedadDeInteresId,
            estimacionCosto);

        await _leadRepository.AddAsync(lead, cancellationToken);

        return new CreateLeadResult(lead.Id.Value, lead.Estado.ToString(), lead.ResultadoCalculadora);
    }
}
