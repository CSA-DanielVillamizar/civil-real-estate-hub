using MediatR;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Application.Properties.Commands.CrearPropiedad;

// Acción administrativa (mismo API key que ConfirmarPagoViabilidadAmbiental
// — ver AdminApiKeyEndpointFilter en WebApi). Recibe primitivos, igual que
// el resto de comandos del proyecto; los Value Objects se construyen en el
// handler. RetirosAmbientales es opcional — una propiedad puede no lindar
// con ninguna fuente hídrica/vía/línea de alta tensión.
public sealed record CrearPropiedadCommand(
    string Titulo,
    string Descripcion,
    TipoInmueble TipoInmueble,
    decimal Precio,
    string Moneda,
    string Direccion,
    string Municipio,
    string Departamento,
    decimal? Latitud,
    decimal? Longitud,
    decimal AreaTerrenoValor,
    UnidadMedidaArea AreaTerrenoUnidad,
    decimal? AreaConstruidaValor,
    UnidadMedidaArea? AreaConstruidaUnidad,
    decimal PendientePorcentaje,
    TipoSuelo TipoSuelo,
    Topografia Topografia,
    decimal? NivelFreaticoMetros,
    IReadOnlyList<RetiroAmbientalInput>? RetirosAmbientales
) : IRequest<CrearPropiedadResult>;

public sealed record RetiroAmbientalInput(TipoFuenteRetiro TipoFuente, decimal DistanciaMinimaMetros, string NormativaAplicable);
