using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Confianza.Commands.DespublicarContenidoConfianza;
using Plataforma.Domain.Confianza;
using Plataforma.Domain.Leads;
using Xunit;

namespace Plataforma.Application.Tests.Confianza.Commands.DespublicarContenidoConfianza;

public sealed class DespublicarContenidoConfianzaCommandHandlerTests
{
    private readonly Mock<IContenidoConfianzaRepository> _repositoryMock = new();
    private readonly DespublicarContenidoConfianzaCommandHandler _sut;

    public DespublicarContenidoConfianzaCommandHandlerTests()
    {
        _sut = new DespublicarContenidoConfianzaCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConContenidoPublicado_LoDespublicaYPersiste()
    {
        var contenido = ContenidoConfianza.Crear(
            TipoContenidoConfianza.Testimonio, "Ana Restrepo", "Excelente.", null, ServicioDeInteres.Inmobiliaria);
        contenido.Publicar();
        _repositoryMock.Setup(r => r.GetByIdAsync(contenido.Id, It.IsAny<CancellationToken>())).ReturnsAsync(contenido);

        var resultado = await _sut.Handle(new DespublicarContenidoConfianzaCommand(contenido.Id.Value), CancellationToken.None);

        resultado!.Publicado.Should().BeFalse();
        _repositoryMock.Verify(r => r.UpdateAsync(contenido, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConContenidoInexistente_DevuelveNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(new ContenidoConfianzaId(id), It.IsAny<CancellationToken>())).ReturnsAsync((ContenidoConfianza?)null);

        var resultado = await _sut.Handle(new DespublicarContenidoConfianzaCommand(id), CancellationToken.None);

        resultado.Should().BeNull();
    }
}
