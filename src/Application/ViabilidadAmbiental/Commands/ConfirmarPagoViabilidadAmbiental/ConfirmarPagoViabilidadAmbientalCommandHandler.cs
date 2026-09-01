using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.ViabilidadAmbiental;

namespace Plataforma.Application.ViabilidadAmbiental.Commands.ConfirmarPagoViabilidadAmbiental;

public sealed class ConfirmarPagoViabilidadAmbientalCommandHandler
    : IRequestHandler<ConfirmarPagoViabilidadAmbientalCommand, ConfirmarPagoViabilidadAmbientalResult?>
{
    private readonly ISolicitudViabilidadAmbientalRepository _repository;

    public ConfirmarPagoViabilidadAmbientalCommandHandler(ISolicitudViabilidadAmbientalRepository repository)
    {
        _repository = repository;
    }

    // Devuelve null si la solicitud no existe — el endpoint lo traduce a 404
    // (no es un invariante de negocio violado, así que no amerita una
    // DomainException; ver LeadsEndpoints/comentario equivalente ausente hoy
    // porque no había antes un caso "no encontrado" expuesto por HTTP).
    public async Task<ConfirmarPagoViabilidadAmbientalResult?> Handle(
        ConfirmarPagoViabilidadAmbientalCommand request, CancellationToken cancellationToken)
    {
        var solicitud = await _repository.GetByIdAsync(new SolicitudViabilidadAmbientalId(request.SolicitudId), cancellationToken);
        if (solicitud is null)
            return null;

        solicitud.ConfirmarPago();
        await _repository.UpdateAsync(solicitud, cancellationToken);

        return new ConfirmarPagoViabilidadAmbientalResult(solicitud.Id.Value, solicitud.Estado.ToString(), solicitud.PagoConfirmadoEn!.Value);
    }
}
