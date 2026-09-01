namespace Plataforma.Application.Common.Interfaces;

// Puerto hacia Azure Blob Storage (Infrastructure). Solo sube el archivo y
// devuelve la URL pública — grabar esa URL en el agregado Propiedad
// (Propiedad.AgregarMultimedia) es responsabilidad del command handler, no
// de este servicio.
public interface IPropertyImageStorage
{
    Task<string> SubirAsync(Stream contenido, string nombreArchivo, string contentType, CancellationToken cancellationToken);
}
