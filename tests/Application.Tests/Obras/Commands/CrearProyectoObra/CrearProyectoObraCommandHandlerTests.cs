using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Obras.Commands.CrearProyectoObra;
using Plataforma.Domain.Obras;
using Xunit;

namespace Plataforma.Application.Tests.Obras.Commands.CrearProyectoObra;

public sealed class CrearProyectoObraCommandHandlerTests
{
    private readonly Mock<IProyectoObraRepository> _repositoryMock = new();
    private readonly CrearProyectoObraCommandHandler _sut;

    public CrearProyectoObraCommandHandlerTests()
    {
        _sut = new CrearProyectoObraCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConDatosValidos_PersisteElProyectoYDevuelveSuToken()
    {
        var command = new CrearProyectoObraCommand(
            "Ana Restrepo", "ana@example.com", "3109876543", null, "Interventoría casa campestre", null, null);

        var resultado = await _sut.Handle(command, CancellationToken.None);

        resultado.Id.Should().NotBeEmpty();
        resultado.TokenAcceso.Should().NotBeNullOrWhiteSpace();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ProyectoObra>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConPropiedadId_LaAsignaAlProyectoPersistido()
    {
        var propiedadId = Guid.NewGuid();
        var command = new CrearProyectoObraCommand(
            "Ana Restrepo", "ana@example.com", "3109876543", null, "Construcción lote", null, propiedadId);

        await _sut.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<ProyectoObra>(p => p.PropiedadId!.Value.Value == propiedadId),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
