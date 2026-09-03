using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Confianza;

public sealed record ActualizarContenidoConfianzaRequest(
    string Titulo,
    string Descripcion,
    string? Municipio,
    ServicioDeInteresDto ServicioRelacionado);
