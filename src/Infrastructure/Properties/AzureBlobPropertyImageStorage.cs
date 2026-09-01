using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Plataforma.Application.Common.Interfaces;

namespace Plataforma.Infrastructure.Properties;

// El contenedor se crea con acceso público de lectura a nivel de blob (ver
// Bicep — publicAccess: 'Blob') a propósito: son fotos de un catálogo
// inmobiliario público, se sirven directo en <img> sin SAS token. Nunca
// contienen datos sensibles.
public sealed class AzureBlobPropertyImageStorage : IPropertyImageStorage
{
    private readonly BlobContainerClient _containerClient;

    public AzureBlobPropertyImageStorage(BlobContainerClient containerClient)
    {
        _containerClient = containerClient;
    }

    public async Task<string> SubirAsync(Stream contenido, string nombreArchivo, string contentType, CancellationToken cancellationToken)
    {
        // Prefijo GUID: evita colisiones entre archivos con el mismo nombre
        // subidos para propiedades distintas, sin depender de que el cliente
        // envíe nombres únicos.
        var extension = Path.GetExtension(nombreArchivo);
        var blobName = $"{Guid.NewGuid()}{extension}";
        var blobClient = _containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(
            contenido,
            new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } },
            cancellationToken);

        return blobClient.Uri.ToString();
    }
}
