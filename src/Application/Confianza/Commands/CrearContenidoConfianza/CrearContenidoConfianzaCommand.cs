using MediatR;
using Plataforma.Application.Confianza.Commands.Common;
using Plataforma.Domain.Confianza;
using Plataforma.Domain.Leads;

namespace Plataforma.Application.Confianza.Commands.CrearContenidoConfianza;

public sealed record CrearContenidoConfianzaCommand(
    TipoContenidoConfianza Tipo,
    string Titulo,
    string Descripcion,
    string? Municipio,
    ServicioDeInteres ServicioRelacionado) : IRequest<ContenidoConfianzaResult>;
