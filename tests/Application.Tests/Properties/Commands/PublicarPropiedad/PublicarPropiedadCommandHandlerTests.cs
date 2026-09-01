using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Properties.Commands.PublicarPropiedad;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.Propiedades.Exceptions;
using Plataforma.Domain.Propiedades.ValueObjects;
using Plataforma.Domain.SharedKernel;
using Xunit;

namespace Plataforma.Application.Tests.Properties.Commands.PublicarPropiedad;

public sealed class PublicarPropiedadCommandHandlerTests
{
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly PublicarPropiedadCommandHandler _sut;

    public PublicarPropiedadCommandHandlerTests()
    {
        _sut = new PublicarPropiedadCommandHandler(_propertyRepositoryMock.Object);
    }

    private static Propiedad CrearPropiedadBorrador(bool conMultimedia = true)
    {
        var propiedad = Propiedad.Crear(
            "Lote campestre", "Descripción", TipoInmueble.Lote, Dinero.Crear(150_000_000m),
            Ubicacion.Crear("Vereda La Primavera", "Rionegro", "Antioquia"),
            Area.Crear(1000m),
            CaracteristicasTopograficas.Crear(15m, TipoSuelo.Franco, Topografia.Plana));

        if (conMultimedia)
            propiedad.AgregarMultimedia("https://storage.example.com/foto1.jpg", TipoMultimedia.Foto);

        return propiedad;
    }

    [Fact]
    public async Task Handle_ConPropiedadPublicable_LaPublicaYPersiste()
    {
        var propiedad = CrearPropiedadBorrador();
        _propertyRepositoryMock.Setup(r => r.GetByIdAsync(propiedad.Id, It.IsAny<CancellationToken>())).ReturnsAsync(propiedad);

        var resultado = await _sut.Handle(new PublicarPropiedadCommand(propiedad.Id.Value), CancellationToken.None);

        resultado!.Estado.Should().Be(nameof(EstadoPropiedad.Publicada));
        _propertyRepositoryMock.Verify(r => r.UpdateAsync(propiedad, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_SinMultimedia_LanzaPropiedadNoPublicableException()
    {
        var propiedad = CrearPropiedadBorrador(conMultimedia: false);
        _propertyRepositoryMock.Setup(r => r.GetByIdAsync(propiedad.Id, It.IsAny<CancellationToken>())).ReturnsAsync(propiedad);

        var act = () => _sut.Handle(new PublicarPropiedadCommand(propiedad.Id.Value), CancellationToken.None);

        await act.Should().ThrowAsync<PropiedadNoPublicableException>();
        _propertyRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Propiedad>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ConPropiedadInexistente_DevuelveNull()
    {
        var propiedadId = Guid.NewGuid();
        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(new PropiedadId(propiedadId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Propiedad?)null);

        var resultado = await _sut.Handle(new PublicarPropiedadCommand(propiedadId), CancellationToken.None);

        resultado.Should().BeNull();
    }
}
