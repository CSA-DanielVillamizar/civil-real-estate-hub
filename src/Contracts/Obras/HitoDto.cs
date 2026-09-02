using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Obras;

public sealed record HitoDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    int Orden,
    EstadoHitoDto Estado,
    DateOnly? FechaEstimada,
    DateTimeOffset? FechaCompletado,
    string? FotoEvidenciaUrl);
