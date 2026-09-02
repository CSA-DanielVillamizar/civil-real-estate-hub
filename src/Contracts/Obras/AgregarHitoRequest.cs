namespace Plataforma.Contracts.Obras;

public sealed record AgregarHitoRequest(string Nombre, string? Descripcion, DateOnly? FechaEstimada);
