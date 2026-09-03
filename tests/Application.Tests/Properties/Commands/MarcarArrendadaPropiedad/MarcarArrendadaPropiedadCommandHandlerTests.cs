using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Properties.Commands.MarcarArrendadaPropiedad;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.Propiedades.Exceptions;
using Plataforma.Domain.Propiedades.ValueObjects;
using Plataforma.Domain.SharedKernel;
using Xunit;

namespace Plataforma.Application.Tests.Properties.Commands.MarcarArrendadaPropiedad;

public sealed class MarcarArrendadaPropiedadCommandHandlerTests
{
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly MarcarArrendadaPropiedadCommandHandler _sut;

    public MarcarArrendadaPropiedadCommandHandlerTests()
    {
        _sut = new MarcarArrendadaPropiedadCommandHandler(_propertyRepositoryMock.Object);
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
    public async Task Handle_ConPropiedadPublicada_LaMarcaArrendadaYPersiste()
    {
        var propiedad = CrearPropiedadPublicada();
        _propertyRepositoryMock.Setup(r => r.GetByIdAsync(propiedad.Id, It.IsAny<CancellationToken>())).ReturnsAsync(propiedad);

        var resultado = await _sut.Handle(new MarcarArrendadaPropiedadCommand(propiedad.Id.Value), CancellationToken.None);

        resultado!.Estado.Should().Be(nameof(EstadoPropiedad.Arrendada));
        _propertyRepositoryMock.Verify(r => r.UpdateAsync(propiedad, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConPropiedadVendida_LanzaPropiedadEnEstadoInvalidoException()
    {
        var propiedad = CrearPropiedadPublicada();
        propiedad.MarcarVendida();
        _propertyRepositoryMock.Setup(r => r.GetByIdAsync(propiedad.Id, It.IsAny<CancellationToken>())).ReturnsAsync(propiedad);

        var accion = () => _sut.Handle(new MarcarArrendadaPropiedadCommand(propiedad.Id.Value), CancellationToken.None);

        await accion.Should().ThrowAsync<PropiedadEnEstadoInvalidoException>();
    }

    [Fact]
    public async Task Handle_ConPropiedadInexistente_DevuelveNull()
    {
        var id = Guid.NewGuid();
        _propertyRepositoryMock.Setup(r => r.GetByIdAsync(new PropiedadId(id), It.IsAny<CancellationToken>())).ReturnsAsync((Propiedad?)null);

        var resultado = await _sut.Handle(new MarcarArrendadaPropiedadCommand(id), CancellationToken.None);

        resultado.Should().BeNull();
    }
}
