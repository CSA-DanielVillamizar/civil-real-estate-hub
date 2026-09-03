using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Confianza.Commands.CrearContenidoConfianza;
using Plataforma.Domain.Confianza;
using Plataforma.Domain.Leads;
using Xunit;

namespace Plataforma.Application.Tests.Confianza.Commands.CrearContenidoConfianza;

public sealed class CrearContenidoConfianzaCommandHandlerTests
{
    private readonly Mock<IContenidoConfianzaRepository> _repositoryMock = new();
    private readonly CrearContenidoConfianzaCommandHandler _sut;

    public CrearContenidoConfianzaCommandHandlerTests()
    {
        _sut = new CrearContenidoConfianzaCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConDatosValidos_CreaSinPublicarYPersiste()
    {
        var comando = new CrearContenidoConfianzaCommand(
            TipoContenidoConfianza.Testimonio, "Ana Restrepo", "Excelente trabajo.", "Rionegro", ServicioDeInteres.ConsultoriaYDisenoEstructural);

        var resultado = await _sut.Handle(comando, CancellationToken.None);

        resultado.Tipo.Should().Be(nameof(TipoContenidoConfianza.Testimonio));
        resultado.Titulo.Should().Be("Ana Restrepo");
        resultado.Publicado.Should().BeFalse();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ContenidoConfianza>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
