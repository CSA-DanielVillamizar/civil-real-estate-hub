using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Tarifas.Commands.CrearPaqueteTarifa;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Tarifas;
using Xunit;

namespace Plataforma.Application.Tests.Tarifas.Commands.CrearPaqueteTarifa;

public sealed class CrearPaqueteTarifaCommandHandlerTests
{
    private readonly Mock<IPaqueteTarifaRepository> _repositoryMock = new();
    private readonly CrearPaqueteTarifaCommandHandler _sut;

    public CrearPaqueteTarifaCommandHandlerTests()
    {
        _sut = new CrearPaqueteTarifaCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConDatosValidos_CreaSinPublicarYPersiste()
    {
        var comando = new CrearPaqueteTarifaCommand(
            ServicioDeInteres.ConsultoriaYDisenoEstructural, "Diseño estructural", "Incluye planos.", 50_000, 80_000, "por m²", "COP");

        var resultado = await _sut.Handle(comando, CancellationToken.None);

        resultado.Titulo.Should().Be("Diseño estructural");
        resultado.PrecioDesde.Should().Be(50_000);
        resultado.Publicado.Should().BeFalse();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<PaqueteTarifa>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
