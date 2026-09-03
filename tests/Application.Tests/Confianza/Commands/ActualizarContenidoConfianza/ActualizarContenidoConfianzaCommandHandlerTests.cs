using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Confianza.Commands.ActualizarContenidoConfianza;
using Plataforma.Domain.Confianza;
using Plataforma.Domain.Leads;
using Xunit;

namespace Plataforma.Application.Tests.Confianza.Commands.ActualizarContenidoConfianza;

public sealed class ActualizarContenidoConfianzaCommandHandlerTests
{
    private readonly Mock<IContenidoConfianzaRepository> _repositoryMock = new();
    private readonly ActualizarContenidoConfianzaCommandHandler _sut;

    public ActualizarContenidoConfianzaCommandHandlerTests()
    {
        _sut = new ActualizarContenidoConfianzaCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConContenidoExistente_ActualizaYPersiste()
    {
        var contenido = ContenidoConfianza.Crear(
            TipoContenidoConfianza.Portafolio, "Título viejo", "Descripción vieja.", "La Ceja", ServicioDeInteres.Inmobiliaria);
        _repositoryMock.Setup(r => r.GetByIdAsync(contenido.Id, It.IsAny<CancellationToken>())).ReturnsAsync(contenido);

        var comando = new ActualizarContenidoConfianzaCommand(
            contenido.Id.Value, "Título nuevo", "Descripción nueva.", "Guarne", ServicioDeInteres.InterventoriaYPresupuestos);

        var resultado = await _sut.Handle(comando, CancellationToken.None);

        resultado!.Titulo.Should().Be("Título nuevo");
        resultado.Municipio.Should().Be("Guarne");
        _repositoryMock.Verify(r => r.UpdateAsync(contenido, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConContenidoInexistente_DevuelveNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(new ContenidoConfianzaId(id), It.IsAny<CancellationToken>())).ReturnsAsync((ContenidoConfianza?)null);

        var resultado = await _sut.Handle(
            new ActualizarContenidoConfianzaCommand(id, "Título", "Descripción.", null, ServicioDeInteres.Inmobiliaria), CancellationToken.None);

        resultado.Should().BeNull();
    }
}
