using Plataforma.Domain.Obras;

namespace Plataforma.Application.Obras.Queries.Common;

public sealed record HitoItem(
    Guid Id,
    string Nombre,
    string? Descripcion,
    int Orden,
    EstadoHito Estado,
    DateOnly? FechaEstimada,
    DateTimeOffset? FechaCompletado,
    string? FotoEvidenciaUrl);

// Compartido entre la consulta admin (por id) y la del cliente (por token) —
// misma forma de datos, solo cambia cómo se llega hasta el agregado.
public sealed record ProyectoObraDetalle(
    Guid Id,
    string NombreCliente,
    string EmailCliente,
    string TelefonoCliente,
    string NombreProyecto,
    string? Descripcion,
    Guid? PropiedadId,
    string TokenAcceso,
    EstadoProyecto Estado,
    DateTimeOffset CreadoEn,
    IReadOnlyList<HitoItem> Hitos);
