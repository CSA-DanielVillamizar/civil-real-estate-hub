using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Obras;

public sealed record ProyectoObraDetalleDto(
    Guid Id,
    string NombreCliente,
    string EmailCliente,
    string TelefonoCliente,
    string NombreProyecto,
    string? Descripcion,
    Guid? PropiedadId,
    EstadoProyectoDto Estado,
    DateTimeOffset CreadoEn,
    IReadOnlyList<HitoDto> Hitos);
