using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Properties.Commands.CrearPropiedad;
using Plataforma.Domain.Propiedades;
using Xunit;

namespace Plataforma.Application.Tests.Properties.Commands.CrearPropiedad;

public sealed class CrearPropiedadCommandHandlerTests
{
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly CrearPropiedadCommandHandler _sut;

    public CrearPropiedadCommandHandlerTests()
    {
        _sut = new CrearPropiedadCommandHandler(_propertyRepositoryMock.Object);
    }

    private static CrearPropiedadCommand ComandoValido(IReadOnlyList<RetiroAmbientalInput>? retiros = null) => new(
        "Lote campestre", "Lote con vista a la montaña", TipoInmueble.Lote, 150_000_000m, "COP",
        "Vereda La Primavera", "Rionegro", "Antioquia", 6.15m, -75.37m,
        1000m, UnidadMedidaArea.M2, null, null,
        15m, TipoSuelo.Franco, Topografia.Plana, 3.5m, retiros);

    [Fact]
    public async Task Handle_ConDatosValidos_PersisteLaPropiedadEnBorrador()
    {
        var resultado = await _sut.Handle(ComandoValido(), CancellationToken.None);

        _propertyRepositoryMock.Verify(r => r.AddAsync(
            It.Is<Propiedad>(p =>
                p.Titulo == "Lote campestre" &&
                p.Estado == EstadoPropiedad.Borrador &&
                p.Ubicacion.Municipio == "Rionegro" &&
                p.Ubicacion.Coordenadas!.Latitud == 6.15m),
            It.IsAny<CancellationToken>()),
            Times.Once);

        resultado.Estado.Should().Be(nameof(EstadoPropiedad.Borrador));
    }

    [Fact]
    public async Task Handle_ConRetirosAmbientales_LosAgregaAlAgregado()
    {
        var retiros = new List<RetiroAmbientalInput>
        {
            new(TipoFuenteRetiro.Rio, 15m, "POT Rionegro - Art. 45"),
        };

        await _sut.Handle(ComandoValido(retiros), CancellationToken.None);

        _propertyRepositoryMock.Verify(r => r.AddAsync(
            It.Is<Propiedad>(p => p.RetirosAmbientales.Count == 1 && p.RetirosAmbientales.First().TipoFuente == TipoFuenteRetiro.Rio),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_SinCoordenadas_CreaLaUbicacionSinCoordenadas()
    {
        var command = ComandoValido() with { Latitud = null, Longitud = null };

        await _sut.Handle(command, CancellationToken.None);

        _propertyRepositoryMock.Verify(r => r.AddAsync(
            It.Is<Propiedad>(p => p.Ubicacion.Coordenadas == null),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
