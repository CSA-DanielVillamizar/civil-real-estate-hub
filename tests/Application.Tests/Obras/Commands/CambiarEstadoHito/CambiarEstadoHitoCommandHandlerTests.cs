using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Obras.Commands.CambiarEstadoHito;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Obras;
using Plataforma.Domain.Obras.Exceptions;
using Xunit;

namespace Plataforma.Application.Tests.Obras.Commands.CambiarEstadoHito;

public sealed class CambiarEstadoHitoCommandHandlerTests
{
    private readonly Mock<IProyectoObraRepository> _repositoryMock = new();
    private readonly CambiarEstadoHitoCommandHandler _sut;

    public CambiarEstadoHitoCommandHandlerTests()
    {
        _sut = new CambiarEstadoHitoCommandHandler(_repositoryMock.Object);
    }

    private static ProyectoObra CrearProyectoConUnHito(out Guid hitoId)
    {
        var proyecto = ProyectoObra.Crear(
            "Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"), "Interventoría casa campestre");
        var hito = proyecto.AgregarHito("Cimentación", null, null);
        hitoId = hito.Id;
        return proyecto;
    }

    [Fact]
    public async Task Handle_ConHitoExistente_CambiaSuEstadoYLoPersiste()
    {
        var proyecto = CrearProyectoConUnHito(out var hitoId);
        _repositoryMock.Setup(r => r.GetByIdAsync(proyecto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(proyecto);

        var resultado = await _sut.Handle(
            new CambiarEstadoHitoCommand(proyecto.Id.Value, hitoId, EstadoHito.Completado), CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado!.Estado.Should().Be(nameof(EstadoHito.Completado));
        _repositoryMock.Verify(r => r.UpdateAsync(proyecto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConProyectoInexistente_DevuelveNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(new ProyectoObraId(id), It.IsAny<CancellationToken>())).ReturnsAsync((ProyectoObra?)null);

        var resultado = await _sut.Handle(new CambiarEstadoHitoCommand(id, Guid.NewGuid(), EstadoHito.Completado), CancellationToken.None);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ConHitoIdInexistenteEnElProyecto_LanzaHitoNoEncontradoException()
    {
        var proyecto = CrearProyectoConUnHito(out _);
        _repositoryMock.Setup(r => r.GetByIdAsync(proyecto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(proyecto);

        var accion = () => _sut.Handle(
            new CambiarEstadoHitoCommand(proyecto.Id.Value, Guid.NewGuid(), EstadoHito.Completado), CancellationToken.None);

        await accion.Should().ThrowAsync<HitoNoEncontradoException>();
    }
}
