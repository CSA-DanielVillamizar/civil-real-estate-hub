using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Tarifas.Commands.DespublicarPaqueteTarifa;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Tarifas;
using Xunit;

namespace Plataforma.Application.Tests.Tarifas.Commands.DespublicarPaqueteTarifa;

public sealed class DespublicarPaqueteTarifaCommandHandlerTests
{
    private readonly Mock<IPaqueteTarifaRepository> _repositoryMock = new();
    private readonly DespublicarPaqueteTarifaCommandHandler _sut;

    public DespublicarPaqueteTarifaCommandHandlerTests()
    {
        _sut = new DespublicarPaqueteTarifaCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConPaquetePublicado_LoDespublicaYPersiste()
    {
        var paquete = PaqueteTarifa.Crear(ServicioDeInteres.Inmobiliaria, "Título", "Descripción.", null, null, "tarifa plana", "COP");
        paquete.Publicar();
        _repositoryMock.Setup(r => r.GetByIdAsync(paquete.Id, It.IsAny<CancellationToken>())).ReturnsAsync(paquete);

        var resultado = await _sut.Handle(new DespublicarPaqueteTarifaCommand(paquete.Id.Value), CancellationToken.None);

        resultado!.Publicado.Should().BeFalse();
        _repositoryMock.Verify(r => r.UpdateAsync(paquete, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConPaqueteInexistente_DevuelveNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(new PaqueteTarifaId(id), It.IsAny<CancellationToken>())).ReturnsAsync((PaqueteTarifa?)null);

        var resultado = await _sut.Handle(new DespublicarPaqueteTarifaCommand(id), CancellationToken.None);

        resultado.Should().BeNull();
    }
}
