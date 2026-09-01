using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.Propiedades.ValueObjects;
using Plataforma.Domain.SharedKernel;

namespace Plataforma.Application.Properties.Commands.CrearPropiedad;

public sealed class CrearPropiedadCommandHandler : IRequestHandler<CrearPropiedadCommand, CrearPropiedadResult>
{
    private readonly IPropertyRepository _propertyRepository;

    public CrearPropiedadCommandHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<CrearPropiedadResult> Handle(CrearPropiedadCommand request, CancellationToken cancellationToken)
    {
        var precio = Dinero.Crear(request.Precio, request.Moneda);

        var coordenadas = request.Latitud.HasValue && request.Longitud.HasValue
            ? Coordenadas.Crear(request.Latitud.Value, request.Longitud.Value)
            : null;

        var ubicacion = Ubicacion.Crear(request.Direccion, request.Municipio, request.Departamento, coordenadas);

        var areaTerreno = Area.Crear(request.AreaTerrenoValor, request.AreaTerrenoUnidad);

        var areaConstruida = request.AreaConstruidaValor.HasValue
            ? Area.Crear(request.AreaConstruidaValor.Value, request.AreaConstruidaUnidad ?? UnidadMedidaArea.M2)
            : null;

        var caracteristicasTopograficas = CaracteristicasTopograficas.Crear(
            request.PendientePorcentaje, request.TipoSuelo, request.Topografia, request.NivelFreaticoMetros);

        var propiedad = Propiedad.Crear(
            request.Titulo,
            request.Descripcion,
            request.TipoInmueble,
            precio,
            ubicacion,
            areaTerreno,
            caracteristicasTopograficas,
            areaConstruida);

        foreach (var retiroInput in request.RetirosAmbientales ?? [])
        {
            var retiro = RetiroAmbiental.Crear(retiroInput.TipoFuente, retiroInput.DistanciaMinimaMetros, retiroInput.NormativaAplicable);
            propiedad.AgregarRetiro(retiro);
        }

        await _propertyRepository.AddAsync(propiedad, cancellationToken);

        return new CrearPropiedadResult(propiedad.Id.Value, propiedad.Estado.ToString());
    }
}
