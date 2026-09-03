using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Confianza;

public sealed record CrearContenidoConfianzaRequest(
    TipoContenidoConfianzaDto Tipo,
    string Titulo,
    string Descripcion,
    string? Municipio,
    ServicioDeInteresDto ServicioRelacionado);
