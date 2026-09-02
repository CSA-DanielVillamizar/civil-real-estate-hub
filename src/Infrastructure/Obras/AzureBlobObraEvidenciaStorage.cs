using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Plataforma.Application.Common.Interfaces;

namespace Plataforma.Infrastructure.Obras;

// El contenedor se crea con acceso público de lectura a nivel de blob (ver
// Bicep — publicAccess: 'Blob'), mismo criterio ya aceptado para
// propiedades-multimedia: se sirve directo en <img> sin SAS token. El
// nombre del blob es un GUID (ver SubirAsync) — tan inadivinable como el
// propio token de acceso al proyecto, así que no supone una fuga real.
//
// Construye su propio BlobContainerClient a partir de ObrasOptions en vez de
// recibirlo inyectado (a diferencia de AzureBlobPropertyImageStorage): dos
// registros de BlobContainerClient sin distinguir por tipo/key en el
// contenedor de DI pisarían uno al otro (el último registrado gana para
// cualquiera de los dos servicios) — más simple evitarlo así que introducir
// DI con keys solo para este caso.
public sealed class AzureBlobObraEvidenciaStorage : IObraEvidenciaStorage
{
    private readonly BlobContainerClient _containerClient;

    public AzureBlobObraEvidenciaStorage(IOptions<ObrasOptions> options)
    {
        var blobServiceClient = new BlobServiceClient(new Uri(options.Value.BlobServiceUri), new DefaultAzureCredential());
        _containerClient = blobServiceClient.GetBlobContainerClient(options.Value.ContainerName);
    }

    public async Task<string> SubirAsync(Stream contenido, string nombreArchivo, string contentType, CancellationToken cancellationToken)
    {
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
