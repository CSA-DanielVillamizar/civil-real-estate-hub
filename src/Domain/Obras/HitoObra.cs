using Plataforma.Domain.Common;

namespace Plataforma.Domain.Obras;

// Entity<Guid> (no un Value Object): tiene identidad propia y su estado
// cambia con el tiempo — mismo patrón que ArchivoMultimedia dentro de
// Propiedad (identidad propia, pero sin repositorio propio, siempre
// accedido a través del agregado ProyectoObra).
public sealed class HitoObra : Entity<Guid>
{
    public string Nombre { get; private set; }
    public string? Descripcion { get; private set; }
    public int Orden { get; private set; }
    public EstadoHito Estado { get; private set; }
    public DateOnly? FechaEstimada { get; private set; }
    public DateTimeOffset? FechaCompletado { get; private set; }

    // Una sola foto de evidencia por hito para esta primera versión — una
    // galería completa por hito es una extensión natural y aditiva si hace
    // falta más adelante, no vale la pena la complejidad de EF Core (colección
    // owned anidada dentro de otra colección owned) para el MVP.
    public string? FotoEvidenciaUrl { get; private set; }

    // Reservado para materialización de EF Core.
    private HitoObra() { }

    private HitoObra(Guid id, string nombre, string? descripcion, int orden, DateOnly? fechaEstimada) : base(id)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        Orden = orden;
        Estado = EstadoHito.Pendiente;
        FechaEstimada = fechaEstimada;
    }

    internal static HitoObra Crear(string nombre, string? descripcion, int orden, DateOnly? fechaEstimada)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del hito es obligatorio.", nameof(nombre));

        return new HitoObra(Guid.NewGuid(), nombre.Trim(), descripcion?.Trim(), orden, fechaEstimada);
    }

    internal void CambiarEstado(EstadoHito nuevoEstado)
    {
        Estado = nuevoEstado;
        // Idempotente: si ya tenía fecha de completado (se marcó Completado
        // más de una vez, o se corrigió el estado ida y vuelta), no la pisa —
        // conserva la primera fecha real en que se completó.
        if (nuevoEstado == EstadoHito.Completado)
            FechaCompletado ??= DateTimeOffset.UtcNow;
    }

    internal void AgregarEvidencia(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("La URL de la evidencia es obligatoria.", nameof(url));

        FotoEvidenciaUrl = url.Trim();
    }
}
