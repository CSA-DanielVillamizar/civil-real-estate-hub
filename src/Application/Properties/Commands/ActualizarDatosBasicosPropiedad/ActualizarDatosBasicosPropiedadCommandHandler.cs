using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.SharedKernel;

namespace Plataforma.Application.Properties.Commands.ActualizarDatosBasicosPropiedad;

public sealed class ActualizarDatosBasicosPropiedadCommandHandler
    : IRequestHandler<ActualizarDatosBasicosPropiedadCommand, ActualizarDatosBasicosPropiedadResult?>
{
    private readonly IPropertyRepository _propertyRepository;

    public ActualizarDatosBasicosPropiedadCommandHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<ActualizarDatosBasicosPropiedadResult?> Handle(
        ActualizarDatosBasicosPropiedadCommand request, CancellationToken cancellationToken)
    {
        var propiedad = await _propertyRepository.GetByIdAsync(new PropiedadId(request.PropiedadId), cancellationToken);
        if (propiedad is null)
            return null;

        var precio = Dinero.Crear(request.Precio, request.Moneda);
        propiedad.ActualizarDatosBasicos(request.Titulo, request.Descripcion, precio);

        await _propertyRepository.UpdateAsync(propiedad, cancellationToken);

        return new ActualizarDatosBasicosPropiedadResult(
            propiedad.Id.Value, propiedad.Titulo, propiedad.Descripcion, propiedad.Precio.Monto, propiedad.Precio.Moneda);
    }
}
