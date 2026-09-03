using MediatR;
using Plataforma.Application.Confianza.Commands.Common;
using Plataforma.Domain.Leads;

namespace Plataforma.Application.Confianza.Commands.ActualizarContenidoConfianza;

public sealed record ActualizarContenidoConfianzaCommand(
    Guid ContenidoId,
    string Titulo,
    string Descripcion,
    string? Municipio,
    ServicioDeInteres ServicioRelacionado) : IRequest<ContenidoConfianzaResult?>;
