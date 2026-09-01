using MediatR;
using Microsoft.Extensions.Logging;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.ViabilidadAmbiental;
using Plataforma.Domain.ViabilidadAmbiental.Services;
using Plataforma.Domain.ViabilidadAmbiental.ValueObjects;

namespace Plataforma.Application.ViabilidadAmbiental.Commands.SolicitarViabilidadAmbiental;

public sealed class SolicitarViabilidadAmbientalCommandHandler
    : IRequestHandler<SolicitarViabilidadAmbientalCommand, SolicitarViabilidadAmbientalResult>
{
    private readonly ISolicitudViabilidadAmbientalRepository _repository;
    private readonly IDatosBancariosProvider _datosBancariosProvider;
    private readonly IEmailSolicitudViabilidadAmbientalService _emailService;
    private readonly ILogger<SolicitarViabilidadAmbientalCommandHandler> _logger;

    public SolicitarViabilidadAmbientalCommandHandler(
        ISolicitudViabilidadAmbientalRepository repository,
        IDatosBancariosProvider datosBancariosProvider,
        IEmailSolicitudViabilidadAmbientalService emailService,
        ILogger<SolicitarViabilidadAmbientalCommandHandler> logger)
    {
        _repository = repository;
        _datosBancariosProvider = datosBancariosProvider;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<SolicitarViabilidadAmbientalResult> Handle(
        SolicitarViabilidadAmbientalCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Crear(request.Email);
        var telefono = Telefono.Crear(request.Telefono, request.Indicativo);
        var solicitante = DatosSolicitante.Crear(request.Nombre, email, telefono);

        var propiedadId = request.PropiedadId.HasValue
            ? new PropiedadId(request.PropiedadId.Value)
            : (PropiedadId?)null;

        var ubicacionLote = propiedadId is null
            ? UbicacionLote.Crear(request.Departamento!, request.Municipio!, request.DireccionReferencia)
            : null;

        var solicitud = SolicitudViabilidadAmbiental.Solicitar(
            solicitante,
            TarifarioViabilidadAmbiental.MontoEstudio(),
            propiedadId,
            ubicacionLote);

        // Persiste primero: la solicitud debe quedar guardada sin importar lo
        // que pase después con el correo (misma lección de resiliencia que
        // Fase 2 — ver LeadCaptadoEventHandler). AddAsync confirma la
        // transacción internamente (ver ILeadRepository para el mismo patrón).
        await _repository.AddAsync(solicitud, cancellationToken);

        var datosBancarios = _datosBancariosProvider.Obtener();

        try
        {
            await _emailService.EnviarInstruccionesPagoAsync(solicitud, datosBancarios, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // La solicitud ya quedó persistida — el cliente también recibe
            // los datos bancarios en la respuesta HTTP (ver el Result), así
            // que perder el correo no le impide pagar.
            _logger.LogError(ex,
                "No se pudo enviar el correo de instrucciones de pago para la solicitud {SolicitudId} — la solicitud sí quedó registrada.",
                solicitud.Id.Value);
        }

        return new SolicitarViabilidadAmbientalResult(
            solicitud.Id.Value,
            solicitud.Estado.ToString(),
            solicitud.Monto.Monto,
            solicitud.Monto.Moneda,
            datosBancarios);
    }
}
