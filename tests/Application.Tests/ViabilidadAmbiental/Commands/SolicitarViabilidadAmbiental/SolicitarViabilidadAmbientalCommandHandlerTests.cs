using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Plataforma.Application.Common;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.ViabilidadAmbiental.Commands.SolicitarViabilidadAmbiental;
using Plataforma.Domain.ViabilidadAmbiental;
using Xunit;

namespace Plataforma.Application.Tests.ViabilidadAmbiental.Commands.SolicitarViabilidadAmbiental;

public sealed class SolicitarViabilidadAmbientalCommandHandlerTests
{
    private readonly Mock<ISolicitudViabilidadAmbientalRepository> _repositoryMock = new();
    private readonly Mock<IDatosBancariosProvider> _datosBancariosProviderMock = new();
    private readonly Mock<IEmailSolicitudViabilidadAmbientalService> _emailServiceMock = new();
    private readonly Mock<ILogger<SolicitarViabilidadAmbientalCommandHandler>> _loggerMock = new();
    private readonly SolicitarViabilidadAmbientalCommandHandler _sut;

    private static readonly DatosBancarios DatosBancariosDePrueba = new("Bancolombia", "Ahorros", "12345678", "Plataforma SAS", "https://example.com/qr.png");

    public SolicitarViabilidadAmbientalCommandHandlerTests()
    {
        _datosBancariosProviderMock.Setup(p => p.Obtener()).Returns(DatosBancariosDePrueba);

        _sut = new SolicitarViabilidadAmbientalCommandHandler(
            _repositoryMock.Object,
            _datosBancariosProviderMock.Object,
            _emailServiceMock.Object,
            _loggerMock.Object);
    }

    private static SolicitarViabilidadAmbientalCommand ComandoConUbicacion() => new(
        "Ana Restrepo", "ana@example.com", "3109876543", "+57",
        PropiedadId: null, Departamento: "Antioquia", Municipio: "Rionegro", DireccionReferencia: "Vereda La Primavera");

    [Fact]
    public async Task Handle_ConUbicacionDeLote_PersisteLaSolicitudYEnviaElCorreo()
    {
        var resultado = await _sut.Handle(ComandoConUbicacion(), CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<SolicitudViabilidadAmbiental>(s =>
                s.Solicitante.Nombre == "Ana Restrepo" &&
                s.Estado == EstadoSolicitudViabilidad.Solicitada &&
                s.UbicacionLote!.Municipio == "Rionegro"),
            It.IsAny<CancellationToken>()),
            Times.Once);

        _emailServiceMock.Verify(s => s.EnviarInstruccionesPagoAsync(
            It.IsAny<SolicitudViabilidadAmbiental>(), DatosBancariosDePrueba, It.IsAny<CancellationToken>()),
            Times.Once);

        resultado.Estado.Should().Be(nameof(EstadoSolicitudViabilidad.Solicitada));
        resultado.DatosBancarios.Should().Be(DatosBancariosDePrueba);
    }

    [Fact]
    public async Task Handle_ConPropiedadId_PersisteLaSolicitudSinUbicacionLote()
    {
        var propiedadId = Guid.NewGuid();
        var command = new SolicitarViabilidadAmbientalCommand(
            "Ana Restrepo", "ana@example.com", "3109876543", null,
            propiedadId, null, null, null);

        await _sut.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<SolicitudViabilidadAmbiental>(s => s.PropiedadId!.Value.Value == propiedadId && s.UbicacionLote == null),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // Misma lección de resiliencia que LeadCaptadoEventHandler (Fase 2): un
    // fallo del correo no debe impedir que la solicitud quede registrada ni
    // que el cliente reciba los datos bancarios en la respuesta HTTP.
    [Fact]
    public async Task Handle_ConElCorreoFallando_NoPropagaLaExcepcionYDevuelveElResultado()
    {
        _emailServiceMock
            .Setup(s => s.EnviarInstruccionesPagoAsync(It.IsAny<SolicitudViabilidadAmbiental>(), It.IsAny<DatosBancarios>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Azure Communication Services no respondió."));

        var resultado = await _sut.Handle(ComandoConUbicacion(), CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado.DatosBancarios.Should().Be(DatosBancariosDePrueba);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<SolicitudViabilidadAmbiental>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DevuelveElMontoDelTarifarioPlaceholder()
    {
        var resultado = await _sut.Handle(ComandoConUbicacion(), CancellationToken.None);

        resultado.Monto.Should().Be(200_000m);
        resultado.Moneda.Should().Be("COP");
    }
}
