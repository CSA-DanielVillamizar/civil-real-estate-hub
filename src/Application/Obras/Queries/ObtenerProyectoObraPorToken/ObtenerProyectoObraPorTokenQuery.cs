using MediatR;
using Plataforma.Application.Obras.Queries.Common;

namespace Plataforma.Application.Obras.Queries.ObtenerProyectoObraPorToken;

// Público — el token ES la credencial (ver ProyectoObra.GenerarToken): no
// hay ningún otro chequeo de autorización en este endpoint.
public sealed record ObtenerProyectoObraPorTokenQuery(string Token) : IRequest<ProyectoObraDetalle?>;
