using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Properties.Commands.AgregarMultimediaAPropiedad;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.Propiedades.ValueObjects;
using Plataforma.Domain.SharedKernel;
using Xunit;

namespace Plataforma.Application.Tests.Properties.Commands.AgregarMultimediaAPropiedad;

public sealed class AgregarMultimediaAPropiedadCommandHandlerTests
{
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly Mock<IPropertyImageStorage> _imageStorageMock = new();
    private readonly AgregarMultimediaAPropiedadCommandHandler _sut;

    public AgregarMultimediaAPropiedadCommandHandlerTests()
    {
        _sut = new AgregarMultimediaAPropiedadCommandHandler(_propertyRepositoryMock.Object, _imageStorageMock.Object);
    }

    private static Propiedad CrearPropiedad() => Propiedad.Crear(
        "Lote campestre", "Descripción", TipoInmueble.Lote, Dinero.Crear(150_000_000m),
        Ubicacion.Crear("Vereda La Primavera", "Rionegro", "Antioquia"),
        Area.Crear(1000m),
        CaracteristicasTopograficas.Crear(15m, TipoSuelo.Franco, Topografia.Plana));

    [Fact]
    public async Task Handle_SubeElArchivoYLoAgregaAlAgregado()
    {
        var propiedad = CrearPropiedad();
        _propertyRepositoryMock.Setup(r => r.GetByIdAsync(propiedad.Id, It.IsAny<CancellationToken>())).ReturnsAsync(propiedad);
        _imageStorageMock
            .Setup(s => s.SubirAsync(It.IsAny<Stream>(), "foto.jpg", "image/jpeg", It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://storage.example.com/abc.jpg");

        using var stream = new MemoryStream();
        var resultado = await _sut.Handle(
            new AgregarMultimediaAPropiedadCommand(propiedad.Id.Value, stream, "foto.jpg", "image/jpeg", TipoMultimedia.Foto),
            CancellationToken.None);

        resultado!.Url.Should().Be("https://storage.example.com/abc.jpg");
        propiedad.Multimedia.Should().ContainSingle(m => m.Url == "https://storage.example.com/abc.jpg");
        _propertyRepositoryMock.Verify(r => r.UpdateAsync(propiedad, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConPropiedadInexistente_DevuelveNullSinSubirNada()
    {
        var propiedadId = Guid.NewGuid();
        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(new PropiedadId(propiedadId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Propiedad?)null);

        using var stream = new MemoryStream();
        var resultado = await _sut.Handle(
            new AgregarMultimediaAPropiedadCommand(propiedadId, stream, "foto.jpg", "image/jpeg", TipoMultimedia.Foto),
            CancellationToken.None);

        resultado.Should().BeNull();
        _imageStorageMock.Verify(s => s.SubirAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
