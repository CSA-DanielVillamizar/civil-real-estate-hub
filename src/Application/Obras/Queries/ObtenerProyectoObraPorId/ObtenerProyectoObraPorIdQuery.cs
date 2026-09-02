using MediatR;
using Plataforma.Application.Obras.Queries.Common;

namespace Plataforma.Application.Obras.Queries.ObtenerProyectoObraPorId;

// Admin — para gestionar hitos de un proyecto puntual.
public sealed record ObtenerProyectoObraPorIdQuery(Guid Id) : IRequest<ProyectoObraDetalle?>;
