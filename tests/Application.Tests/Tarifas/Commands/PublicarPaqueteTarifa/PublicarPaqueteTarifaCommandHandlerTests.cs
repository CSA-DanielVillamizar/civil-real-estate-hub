using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Tarifas.Commands.PublicarPaqueteTarifa;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Tarifas;
using Xunit;

namespace Plataforma.Application.Tests.Tarifas.Commands.PublicarPaqueteTarifa;

public sealed class PublicarPaqueteTarifaCommandHandlerTests
{
    private readonly Mock<IPaqueteTarifaRepository> _repositoryMock = new();
    private readonly PublicarPaqueteTarifaCommandHandler _sut;

    public PublicarPaqueteTarifaCommandHandlerTests()
    {
        _sut = new PublicarPaqueteTarifaCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConPaqueteExistente_LoPublicaYPersiste()
    {
        var paquete = PaqueteTarifa.Crear(ServicioDeInteres.Inmobiliaria, "Título", "Descripción.", null, null, "tarifa plana", "COP");
        _repositoryMock.Setup(r => r.GetByIdAsync(paquete.Id, It.IsAny<CancellationToken>())).ReturnsAsync(paquete);

        var resultado = await _sut.Handle(new PublicarPaqueteTarifaCommand(paquete.Id.Value), CancellationToken.None);

        resultado!.Publicado.Should().BeTrue();
        _repositoryMock.Verify(r => r.UpdateAsync(paquete, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConPaqueteInexistente_DevuelveNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(new PaqueteTarifaId(id), It.IsAny<CancellationToken>())).ReturnsAsync((PaqueteTarifa?)null);

        var resultado = await _sut.Handle(new PublicarPaqueteTarifaCommand(id), CancellationToken.None);

        resultado.Should().BeNull();
    }
}
