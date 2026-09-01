using Plataforma.Application.Common;
using Plataforma.Domain.ViabilidadAmbiental;

namespace Plataforma.Application.Common.Interfaces;

public interface IEmailSolicitudViabilidadAmbientalService
{
    Task EnviarInstruccionesPagoAsync(
        SolicitudViabilidadAmbiental solicitud,
        DatosBancarios datosBancarios,
        CancellationToken cancellationToken);
}
