using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Confianza.Commands.PublicarContenidoConfianza;
using Plataforma.Domain.Confianza;
using Plataforma.Domain.Leads;
using Xunit;

namespace Plataforma.Application.Tests.Confianza.Commands.PublicarContenidoConfianza;

public sealed class PublicarContenidoConfianzaCommandHandlerTests
{
    private readonly Mock<IContenidoConfianzaRepository> _repositoryMock = new();
    private readonly PublicarContenidoConfianzaCommandHandler _sut;

    public PublicarContenidoConfianzaCommandHandlerTests()
    {
        _sut = new PublicarContenidoConfianzaCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConContenidoExistente_LoPublicaYPersiste()
    {
        var contenido = ContenidoConfianza.Crear(
            TipoContenidoConfianza.Testimonio, "Ana Restrepo", "Excelente.", null, ServicioDeInteres.Inmobiliaria);
        _repositoryMock.Setup(r => r.GetByIdAsync(contenido.Id, It.IsAny<CancellationToken>())).ReturnsAsync(contenido);

        var resultado = await _sut.Handle(new PublicarContenidoConfianzaCommand(contenido.Id.Value), CancellationToken.None);

        resultado!.Publicado.Should().BeTrue();
        _repositoryMock.Verify(r => r.UpdateAsync(contenido, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConContenidoInexistente_DevuelveNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(new ContenidoConfianzaId(id), It.IsAny<CancellationToken>())).ReturnsAsync((ContenidoConfianza?)null);

        var resultado = await _sut.Handle(new PublicarContenidoConfianzaCommand(id), CancellationToken.None);

        resultado.Should().BeNull();
    }
}
