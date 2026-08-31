using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Leads.Services;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Application.Leads.Commands.GenerarPresupuestoPdf;

public sealed class GenerarPresupuestoPdfCommandHandler : IRequestHandler<GenerarPresupuestoPdfCommand, GenerarPresupuestoPdfResult>
{
    private readonly ILeadRepository _leadRepository;
    private readonly CalculadoraDeObraService _calculadoraDeObraService;
    private readonly IPresupuestoPdfGenerator _pdfGenerator;

    public GenerarPresupuestoPdfCommandHandler(
        ILeadRepository leadRepository,
        CalculadoraDeObraService calculadoraDeObraService,
        IPresupuestoPdfGenerator pdfGenerator)
    {
        _leadRepository = leadRepository;
        _calculadoraDeObraService = calculadoraDeObraService;
        _pdfGenerator = pdfGenerator;
    }

    public async Task<GenerarPresupuestoPdfResult> Handle(GenerarPresupuestoPdfCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Crear(request.Email);
        var telefono = Telefono.Crear(request.Telefono, request.Indicativo);

        // GenerarPresupuestoPdfCommandValidator ya garantizó (vía NotNull())
        // que el pipeline de MediatR no llega hasta aquí si viene null.
        var input = request.DatosCalculoObra!;
        var datosCalculoObra = DatosCalculoObra.Crear(
            input.AreaConstruccionM2,
            input.TipoAcabado,
            input.Municipio,
            input.TipoProyecto);

        var estimacionCosto = _calculadoraDeObraService.Calcular(datosCalculoObra);

        var propiedadDeInteresId = request.PropiedadDeInteresId.HasValue
            ? new PropiedadId(request.PropiedadDeInteresId.Value)
            : (PropiedadId?)null;

        var lead = Lead.Registrar(
            request.Nombre,
            email,
            telefono,
            OrigenLead.CalculadoraObra,
            propiedadDeInteresId,
            estimacionCosto);

        // Descargar el PDF es una señal de intención más fuerte que solo usar
        // la calculadora — el lead nace ya calificado (ver Lead.CalificarPorDescargaDePdf).
        lead.CalificarPorDescargaDePdf();

        await _leadRepository.AddAsync(lead, cancellationToken);

        var pdfBytes = _pdfGenerator.Generar(lead);
        var fileName = $"presupuesto-{lead.Id.Value:N}.pdf";

        return new GenerarPresupuestoPdfResult(lead.Id.Value, lead.Estado.ToString(), pdfBytes, fileName);
    }
}
