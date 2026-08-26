using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades;

public sealed class ArchivoMultimedia : Entity<Guid>
{
    public string Url { get; private set; }
    public TipoMultimedia Tipo { get; private set; }
    public int Orden { get; private set; }

    // Reservado para materialización de EF Core (Fase 4).
    private ArchivoMultimedia() { }

    private ArchivoMultimedia(Guid id, string url, TipoMultimedia tipo, int orden) : base(id)
    {
        Url = url;
        Tipo = tipo;
        Orden = orden;
    }

    internal static ArchivoMultimedia Crear(string url, TipoMultimedia tipo, int orden)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("La URL del archivo multimedia es obligatoria.", nameof(url));

        if (orden < 0)
            throw new ArgumentException("El orden no puede ser negativo.", nameof(orden));

        return new ArchivoMultimedia(Guid.NewGuid(), url.Trim(), tipo, orden);
    }
}
