using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Properties.Commands.ActualizarDatosBasicosPropiedad;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.Propiedades.ValueObjects;
using Plataforma.Domain.SharedKernel;
using Xunit;

namespace Plataforma.Application.Tests.Properties.Commands.ActualizarDatosBasicosPropiedad;

public sealed class ActualizarDatosBasicosPropiedadCommandHandlerTests
{
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly ActualizarDatosBasicosPropiedadCommandHandler _sut;

    public ActualizarDatosBasicosPropiedadCommandHandlerTests()
    {
        _sut = new ActualizarDatosBasicosPropiedadCommandHandler(_propertyRepositoryMock.Object);
    }

    private static Propiedad CrearPropiedad() => Propiedad.Crear(
        "Lote campestre", "Descripción", TipoInmueble.Lote, Dinero.Crear(150_000_000m),
        Ubicacion.Crear("Vereda La Primavera", "Rionegro", "Antioquia"),
        Area.Crear(1000m),
        CaracteristicasTopograficas.Crear(15m, TipoSuelo.Franco, Topografia.Plana));

    [Fact]
    public async Task Handle_ConDatosValidos_ActualizaTituloDescripcionYPrecio()
    {
        var propiedad = CrearPropiedad();
        _propertyRepositoryMock.Setup(r => r.GetByIdAsync(propiedad.Id, It.IsAny<CancellationToken>())).ReturnsAsync(propiedad);

        var resultado = await _sut.Handle(
            new ActualizarDatosBasicosPropiedadCommand(propiedad.Id.Value, "Lote renovado", "Nueva descripción", 180_000_000m, "COP"),
            CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado!.Titulo.Should().Be("Lote renovado");
        resultado.Descripcion.Should().Be("Nueva descripción");
        resultado.Precio.Should().Be(180_000_000m);
        _propertyRepositoryMock.Verify(r => r.UpdateAsync(propiedad, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConPropiedadInexistente_DevuelveNull()
    {
        var id = Guid.NewGuid();
        _propertyRepositoryMock.Setup(r => r.GetByIdAsync(new PropiedadId(id), It.IsAny<CancellationToken>())).ReturnsAsync((Propiedad?)null);

        var resultado = await _sut.Handle(
            new ActualizarDatosBasicosPropiedadCommand(id, "Título", "Descripción", 100_000_000m, "COP"), CancellationToken.None);

        resultado.Should().BeNull();
    }
}
