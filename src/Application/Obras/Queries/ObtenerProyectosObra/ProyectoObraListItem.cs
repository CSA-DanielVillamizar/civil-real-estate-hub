using Plataforma.Domain.Obras;

namespace Plataforma.Application.Obras.Queries.ObtenerProyectosObra;

public sealed record ProyectoObraListItem(
    Guid Id,
    string NombreCliente,
    string NombreProyecto,
    EstadoProyecto Estado,
    DateTimeOffset CreadoEn,
    int TotalHitos,
    int HitosCompletados,
    string TokenAcceso);
