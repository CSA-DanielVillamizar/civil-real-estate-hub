using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.Propiedades.Exceptions;

namespace Plataforma.Application.Properties.Commands.PublicarPropiedad;

public sealed class PublicarPropiedadCommandHandler : IRequestHandler<PublicarPropiedadCommand, PublicarPropiedadResult?>
{
    private readonly IPropertyRepository _propertyRepository;

    public PublicarPropiedadCommandHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PublicarPropiedadResult?> Handle(PublicarPropiedadCommand request, CancellationToken cancellationToken)
    {
        var propiedad = await _propertyRepository.GetByIdAsync(new PropiedadId(request.PropiedadId), cancellationToken);
        if (propiedad is null)
            return null;

        // Propiedad.Publicar() lanza InvalidOperationException (código
        // preexistente de Fase 1-4, anterior a la convención de
        // DomainException adoptada después) — se traduce aquí en el borde de
        // Application para que ApplicationExceptionHandler la mapee a 400 en
        // vez de un 500 genérico, sin tocar el dominio ya probado.
        try
        {
            propiedad.Publicar();
        }
        catch (InvalidOperationException ex)
        {
            throw new PropiedadNoPublicableException(ex.Message);
        }

        await _propertyRepository.UpdateAsync(propiedad, cancellationToken);

        return new PublicarPropiedadResult(propiedad.Id.Value, propiedad.Estado.ToString());
    }
}
