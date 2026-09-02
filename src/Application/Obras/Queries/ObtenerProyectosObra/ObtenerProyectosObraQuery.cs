using MediatR;

namespace Plataforma.Application.Obras.Queries.ObtenerProyectosObra;

public sealed record ObtenerProyectosObraQuery : IRequest<IReadOnlyList<ProyectoObraListItem>>;
