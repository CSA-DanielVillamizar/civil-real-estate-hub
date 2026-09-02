namespace Plataforma.Application.Common.Interfaces;

// Puerto hacia Azure Blob Storage (Infrastructure) — mismo contrato que
// IPropertyImageStorage, contenedor distinto (obras-evidencia).
public interface IObraEvidenciaStorage
{
    Task<string> SubirAsync(Stream contenido, string nombreArchivo, string contentType, CancellationToken cancellationToken);
}
