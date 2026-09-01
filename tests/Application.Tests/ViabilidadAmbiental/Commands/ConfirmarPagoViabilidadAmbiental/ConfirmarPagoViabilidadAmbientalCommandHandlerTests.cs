using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.ViabilidadAmbiental.Commands.ConfirmarPagoViabilidadAmbiental;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.SharedKernel;
using Plataforma.Domain.ViabilidadAmbiental;
using Plataforma.Domain.ViabilidadAmbiental.Exceptions;
using Plataforma.Domain.ViabilidadAmbiental.ValueObjects;
using Xunit;

namespace Plataforma.Application.Tests.ViabilidadAmbiental.Commands.ConfirmarPagoViabilidadAmbiental;

public sealed class ConfirmarPagoViabilidadAmbientalCommandHandlerTests
{
    private readonly Mock<ISolicitudViabilidadAmbientalRepository> _repositoryMock = new();
    private readonly ConfirmarPagoViabilidadAmbientalCommandHandler _sut;

    public ConfirmarPagoViabilidadAmbientalCommandHandlerTests()
    {
        _sut = new ConfirmarPagoViabilidadAmbientalCommandHandler(_repositoryMock.Object);
    }

    private static SolicitudViabilidadAmbiental CrearSolicitud() => SolicitudViabilidadAmbiental.Solicitar(
        DatosSolicitante.Crear("Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543")),
        Dinero.Crear(200_000m),
        ubicacionLote: UbicacionLote.Crear("Antioquia", "Rionegro"));

    [Fact]
    public async Task Handle_ConSolicitudExistente_ConfirmaElPagoYPersiste()
    {
        var solicitud = CrearSolicitud();
        _repositoryMock.Setup(r => r.GetByIdAsync(solicitud.Id, It.IsAny<CancellationToken>())).ReturnsAsync(solicitud);

        var resultado = await _sut.Handle(new ConfirmarPagoViabilidadAmbientalCommand(solicitud.Id.Value), CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado!.Estado.Should().Be(nameof(EstadoSolicitudViabilidad.Pagada));
        solicitud.Estado.Should().Be(EstadoSolicitudViabilidad.Pagada);
        _repositoryMock.Verify(r => r.UpdateAsync(solicitud, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConSolicitudInexistente_DevuelveNull()
    {
        var solicitudId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(new SolicitudViabilidadAmbientalId(solicitudId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SolicitudViabilidadAmbiental?)null);

        var resultado = await _sut.Handle(new ConfirmarPagoViabilidadAmbientalCommand(solicitudId), CancellationToken.None);

        resultado.Should().BeNull();
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SolicitudViabilidadAmbiental>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ConSolicitudYaPagada_LanzaEstadoSolicitudViabilidadInvalidoException()
    {
        var solicitud = CrearSolicitud();
        solicitud.ConfirmarPago();
        _repositoryMock.Setup(r => r.GetByIdAsync(solicitud.Id, It.IsAny<CancellationToken>())).ReturnsAsync(solicitud);

        var act = () => _sut.Handle(new ConfirmarPagoViabilidadAmbientalCommand(solicitud.Id.Value), CancellationToken.None);

        await act.Should().ThrowAsync<EstadoSolicitudViabilidadInvalidoException>();
    }
}
