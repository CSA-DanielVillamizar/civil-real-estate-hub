using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Obras;

public sealed record CambiarEstadoProyectoRequest(EstadoProyectoDto NuevoEstado);
