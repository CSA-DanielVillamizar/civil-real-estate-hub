using Plataforma.Domain.Confianza;

namespace Plataforma.Application.Confianza.Commands.Common;

public sealed record ContenidoConfianzaResult(
    Guid Id,
    string Tipo,
    string Titulo,
    string Descripcion,
    string? Municipio,
    string ServicioRelacionado,
    bool Publicado,
    DateTimeOffset CreadoEn);

// Se reutiliza desde todos los handlers de Commands/Queries de este
// aggregate (crear, actualizar, publicar, despublicar, listar) en vez de
// repetir la misma proyección seis veces.
public static class ContenidoConfianzaMapping
{
    public static ContenidoConfianzaResult ToResult(this ContenidoConfianza contenido) =>
        new(
            contenido.Id.Value,
            contenido.Tipo.ToString(),
            contenido.Titulo,
            contenido.Descripcion,
            contenido.Municipio,
            contenido.ServicioRelacionado.ToString(),
            contenido.Publicado,
            contenido.CreadoEn);
}
