using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Obras;

public sealed record ProyectoObraListItemDto(
    Guid Id,
    string NombreCliente,
    string NombreProyecto,
    EstadoProyectoDto Estado,
    DateTimeOffset CreadoEn,
    int TotalHitos,
    int HitosCompletados,
    string TokenAcceso);
