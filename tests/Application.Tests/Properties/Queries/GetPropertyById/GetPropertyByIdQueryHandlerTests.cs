using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Properties.Queries.GetPropertyById;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.Propiedades.ValueObjects;
using Plataforma.Domain.SharedKernel;
using Xunit;

namespace Plataforma.Application.Tests.Properties.Queries.GetPropertyById;

public sealed class GetPropertyByIdQueryHandlerTests
{
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly GetPropertyByIdQueryHandler _sut;

    public GetPropertyByIdQueryHandlerTests()
    {
        _sut = new GetPropertyByIdQueryHandler(_propertyRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConPropiedadExistente_DevuelveElDetalleConRestriccionesDeViabilidad()
    {
        var propiedad = Propiedad.Crear(
            "Lote campestre", "Descripción", TipoInmueble.Lote, Dinero.Crear(150_000_000m),
            Ubicacion.Crear("Vereda La Primavera", "Rionegro", "Antioquia"),
            Area.Crear(1000m),
            // Pendiente 30% > el máximo de referencia (25%) → no viable.
            CaracteristicasTopograficas.Crear(30m, TipoSuelo.Franco, Topografia.Inclinada));
        propiedad.AgregarRetiro(RetiroAmbiental.Crear(TipoFuenteRetiro.Rio, 15m, "POT Rionegro"));
        _propertyRepositoryMock.Setup(r => r.GetByIdAsync(propiedad.Id, It.IsAny<CancellationToken>())).ReturnsAsync(propiedad);

        var resultado = await _sut.Handle(new GetPropertyByIdQuery(propiedad.Id.Value), CancellationToken.None);

        resultado!.EsViableConstructivamente.Should().BeFalse();
        resultado.RestriccionesViabilidad.Should().HaveCount(2); // pendiente + retiro
        resultado.RetirosAmbientales.Should().ContainSingle(r => r.TipoFuente == TipoFuenteRetiro.Rio);
    }

    [Fact]
    public async Task Handle_ConPropiedadInexistente_DevuelveNull()
    {
        var propiedadId = Guid.NewGuid();
        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(new PropiedadId(propiedadId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Propiedad?)null);

        var resultado = await _sut.Handle(new GetPropertyByIdQuery(propiedadId), CancellationToken.None);

        resultado.Should().BeNull();
    }
}
