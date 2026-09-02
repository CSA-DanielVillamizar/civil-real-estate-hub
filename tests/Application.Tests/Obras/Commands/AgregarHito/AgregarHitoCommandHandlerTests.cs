using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Obras.Commands.AgregarHito;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Obras;
using Xunit;

namespace Plataforma.Application.Tests.Obras.Commands.AgregarHito;

public sealed class AgregarHitoCommandHandlerTests
{
    private readonly Mock<IProyectoObraRepository> _repositoryMock = new();
    private readonly AgregarHitoCommandHandler _sut;

    public AgregarHitoCommandHandlerTests()
    {
        _sut = new AgregarHitoCommandHandler(_repositoryMock.Object);
    }

    private static ProyectoObra CrearProyecto() => ProyectoObra.Crear(
        "Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"), "Interventoría casa campestre");

    [Fact]
    public async Task Handle_ConProyectoExistente_AgregaElHitoYLoPersiste()
    {
        var proyecto = CrearProyecto();
        _repositoryMock.Setup(r => r.GetByIdAsync(proyecto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(proyecto);

        var resultado = await _sut.Handle(
            new AgregarHitoCommand(proyecto.Id.Value, "Cimentación", "Excavación y fundida", null), CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("Cimentación");
        resultado.Estado.Should().Be(nameof(EstadoHito.Pendiente));
        _repositoryMock.Verify(r => r.UpdateAsync(proyecto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConProyectoInexistente_DevuelveNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(new ProyectoObraId(id), It.IsAny<CancellationToken>())).ReturnsAsync((ProyectoObra?)null);

        var resultado = await _sut.Handle(new AgregarHitoCommand(id, "Cimentación", null, null), CancellationToken.None);

        resultado.Should().BeNull();
    }
}
