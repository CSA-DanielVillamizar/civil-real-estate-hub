using MediatR;
using Plataforma.Application.Tarifas.Commands.Common;
using Plataforma.Domain.Leads;

namespace Plataforma.Application.Tarifas.Commands.ActualizarPaqueteTarifa;

public sealed record ActualizarPaqueteTarifaCommand(
    Guid PaqueteId,
    string Titulo,
    string Descripcion,
    decimal? PrecioDesde,
    decimal? PrecioHasta,
    string UnidadPrecio,
    ServicioDeInteres ServicioRelacionado) : IRequest<PaqueteTarifaResult?>;
