using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Tarifas.Commands.ActualizarPaqueteTarifa;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Tarifas;
using Xunit;

namespace Plataforma.Application.Tests.Tarifas.Commands.ActualizarPaqueteTarifa;

public sealed class ActualizarPaqueteTarifaCommandHandlerTests
{
    private readonly Mock<IPaqueteTarifaRepository> _repositoryMock = new();
    private readonly ActualizarPaqueteTarifaCommandHandler _sut;

    public ActualizarPaqueteTarifaCommandHandlerTests()
    {
        _sut = new ActualizarPaqueteTarifaCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConPaqueteExistente_ActualizaYPersiste()
    {
        var paquete = PaqueteTarifa.Crear(ServicioDeInteres.Inmobiliaria, "Viejo", "Descripción vieja.", null, null, "tarifa plana", "COP");
        _repositoryMock.Setup(r => r.GetByIdAsync(paquete.Id, It.IsAny<CancellationToken>())).ReturnsAsync(paquete);

        var comando = new ActualizarPaqueteTarifaCommand(
            paquete.Id.Value, "Nuevo", "Descripción nueva.", 10_000, 20_000, "por m²", ServicioDeInteres.InterventoriaYPresupuestos);

        var resultado = await _sut.Handle(comando, CancellationToken.None);

        resultado!.Titulo.Should().Be("Nuevo");
        resultado.PrecioDesde.Should().Be(10_000);
        _repositoryMock.Verify(r => r.UpdateAsync(paquete, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConPaqueteInexistente_DevuelveNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(new PaqueteTarifaId(id), It.IsAny<CancellationToken>())).ReturnsAsync((PaqueteTarifa?)null);

        var resultado = await _sut.Handle(
            new ActualizarPaqueteTarifaCommand(id, "Título", "Descripción.", null, null, "tarifa plana", ServicioDeInteres.Inmobiliaria),
            CancellationToken.None);

        resultado.Should().BeNull();
    }
}
