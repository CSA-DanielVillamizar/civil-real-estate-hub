using Plataforma.Application.Confianza.Commands.ActualizarContenidoConfianza;
using Plataforma.Application.Confianza.Commands.Common;
using Plataforma.Application.Confianza.Commands.CrearContenidoConfianza;
using Plataforma.Contracts.Confianza;

namespace Plataforma.WebApi.Mapping;

public static class ConfianzaMapping
{
    public static CrearContenidoConfianzaCommand ToCommand(this CrearContenidoConfianzaRequest request) =>
        new(request.Tipo.ToDomain(), request.Titulo, request.Descripcion, request.Municipio, request.ServicioRelacionado.ToDomain());

    public static ActualizarContenidoConfianzaCommand ToCommand(this ActualizarContenidoConfianzaRequest request, Guid contenidoId) =>
        new(contenidoId, request.Titulo, request.Descripcion, request.Municipio, request.ServicioRelacionado.ToDomain());

    public static ContenidoConfianzaDto ToContract(this ContenidoConfianzaResult result) =>
        new(result.Id, result.Tipo, result.Titulo, result.Descripcion, result.Municipio, result.ServicioRelacionado, result.Publicado, result.CreadoEn);
}
