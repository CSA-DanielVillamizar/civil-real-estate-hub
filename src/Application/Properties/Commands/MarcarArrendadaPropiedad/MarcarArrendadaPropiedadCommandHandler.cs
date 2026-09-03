using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Properties.Commands.Common;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.Propiedades.Exceptions;

namespace Plataforma.Application.Properties.Commands.MarcarArrendadaPropiedad;

public sealed class MarcarArrendadaPropiedadCommandHandler : IRequestHandler<MarcarArrendadaPropiedadCommand, PropertyEstadoResult?>
{
    private readonly IPropertyRepository _propertyRepository;

    public MarcarArrendadaPropiedadCommandHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PropertyEstadoResult?> Handle(MarcarArrendadaPropiedadCommand request, CancellationToken cancellationToken)
    {
        var propiedad = await _propertyRepository.GetByIdAsync(new PropiedadId(request.PropiedadId), cancellationToken);
        if (propiedad is null)
            return null;

        try
        {
            propiedad.MarcarArrendada();
        }
        catch (InvalidOperationException ex)
        {
            throw new PropiedadEnEstadoInvalidoException(ex.Message);
        }

        await _propertyRepository.UpdateAsync(propiedad, cancellationToken);
        return new PropertyEstadoResult(propiedad.Id.Value, propiedad.Estado.ToString());
    }
}
