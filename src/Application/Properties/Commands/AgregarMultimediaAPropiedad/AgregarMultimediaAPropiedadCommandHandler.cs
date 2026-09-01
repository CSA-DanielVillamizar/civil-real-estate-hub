using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Application.Properties.Commands.AgregarMultimediaAPropiedad;

public sealed class AgregarMultimediaAPropiedadCommandHandler
    : IRequestHandler<AgregarMultimediaAPropiedadCommand, AgregarMultimediaAPropiedadResult?>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyImageStorage _imageStorage;

    public AgregarMultimediaAPropiedadCommandHandler(IPropertyRepository propertyRepository, IPropertyImageStorage imageStorage)
    {
        _propertyRepository = propertyRepository;
        _imageStorage = imageStorage;
    }

    public async Task<AgregarMultimediaAPropiedadResult?> Handle(
        AgregarMultimediaAPropiedadCommand request, CancellationToken cancellationToken)
    {
        var propiedad = await _propertyRepository.GetByIdAsync(new PropiedadId(request.PropiedadId), cancellationToken);
        if (propiedad is null)
            return null;

        // Sube primero: si Blob Storage falla, no queda un registro de
        // multimedia con una URL inexistente en el agregado.
        var url = await _imageStorage.SubirAsync(request.Contenido, request.NombreArchivo, request.ContentType, cancellationToken);

        propiedad.AgregarMultimedia(url, request.Tipo);
        await _propertyRepository.UpdateAsync(propiedad, cancellationToken);

        return new AgregarMultimediaAPropiedadResult(propiedad.Id.Value, url, request.Tipo.ToString());
    }
}
