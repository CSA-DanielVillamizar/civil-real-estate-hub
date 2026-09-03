using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Properties.Commands.ReservarPropiedad;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.Propiedades.Exceptions;
using Plataforma.Domain.Propiedades.ValueObjects;
using Plataforma.Domain.SharedKernel;
using Xunit;

namespace Plataforma.Application.Tests.Properties.Commands.ReservarPropiedad;

public sealed class ReservarPropiedadCommandHandlerTests
{
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly ReservarPropiedadCommandHandler _sut;

    public ReservarPropiedadCommandHandlerTests()
    {
        _sut = new ReservarPropiedadCommandHandler(_propertyRepositoryMock.Object);
    }

    private static Propiedad CrearPropiedadPublicada()
    {
        var propiedad = Propiedad.Crear(
            "Lote campestre", "Descripción", TipoInmueble.Lote, Dinero.Crear(150_000_000m),
            Ubicacion.Crear("Vereda La Primavera", "Rionegro", "Antioquia"),
            Area.Crear(1000m),
            CaracteristicasTopograficas.Crear(15m, TipoSuelo.Franco, Topografia.Plana));
        propiedad.AgregarMultimedia("https://storage.example.com/foto1.jpg", TipoMultimedia.Foto);
        propiedad.Publicar();
        return propiedad;
    }

    [Fact]
    public async Task Handle_ConPropiedadPublicada_LaReservaYPersiste()
    {
        var propiedad = CrearPropiedadPublicada();
        _propertyRepositoryMock.Setup(r => r.GetByIdAsync(propiedad.Id, It.IsAny<CancellationToken>())).ReturnsAsync(propiedad);

        var resultado = await _sut.Handle(new ReservarPropiedadCommand(propiedad.Id.Value), CancellationToken.None);

        resultado!.Estado.Should().Be(nameof(EstadoPropiedad.Reservada));
        _propertyRepositoryMock.Verify(r => r.UpdateAsync(propiedad, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConPropiedadEnBorrador_LanzaPropiedadEnEstadoInvalidoException()
    {
        var propiedad = Propiedad.Crear(
            "Lote campestre", "Descripción", TipoInmueble.Lote, Dinero.Crear(150_000_000m),
            Ubicacion.Crear("Vereda La Primavera", "Rionegro", "Antioquia"),
            Area.Crear(1000m),
            CaracteristicasTopograficas.Crear(15m, TipoSuelo.Franco, Topografia.Plana));
        _propertyRepositoryMock.Setup(r => r.GetByIdAsync(propiedad.Id, It.IsAny<CancellationToken>())).ReturnsAsync(propiedad);

        var accion = () => _sut.Handle(new ReservarPropiedadCommand(propiedad.Id.Value), CancellationToken.None);

        await accion.Should().ThrowAsync<PropiedadEnEstadoInvalidoException>();
    }

    [Fact]
    public async Task Handle_ConPropiedadInexistente_DevuelveNull()
    {
        var id = Guid.NewGuid();
        _propertyRepositoryMock.Setup(r => r.GetByIdAsync(new PropiedadId(id), It.IsAny<CancellationToken>())).ReturnsAsync((Propiedad?)null);

        var resultado = await _sut.Handle(new ReservarPropiedadCommand(id), CancellationToken.None);

        resultado.Should().BeNull();
    }
}
