using MediatR;
using Plataforma.Application.Tarifas.Commands.Common;
using Plataforma.Domain.Leads;

namespace Plataforma.Application.Tarifas.Commands.CrearPaqueteTarifa;

public sealed record CrearPaqueteTarifaCommand(
    ServicioDeInteres ServicioRelacionado,
    string Titulo,
    string Descripcion,
    decimal? PrecioDesde,
    decimal? PrecioHasta,
    string UnidadPrecio,
    string Moneda) : IRequest<PaqueteTarifaResult>;
