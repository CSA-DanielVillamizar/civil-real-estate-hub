using FluentAssertions;
using Moq;
using Plataforma.Application.Auth.Commands.CambiarActivoUsuario;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Usuarios;
using Xunit;

namespace Plataforma.Application.Tests.Auth.Commands.CambiarActivoUsuario;

public sealed class CambiarActivoUsuarioCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();
    private readonly CambiarActivoUsuarioCommandHandler _sut;

    public CambiarActivoUsuarioCommandHandlerTests()
    {
        _sut = new CambiarActivoUsuarioCommandHandler(_usuarioRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConActivoFalse_DesactivaAlUsuarioYPersiste()
    {
        var usuario = Usuario.Crear("Laura Gómez", Email.Crear("laura@example.com"), "hash", RolUsuario.AsesorComercial);
        _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(usuario.Id, It.IsAny<CancellationToken>())).ReturnsAsync(usuario);

        var resultado = await _sut.Handle(new CambiarActivoUsuarioCommand(usuario.Id.Value, false), CancellationToken.None);

        resultado!.Activo.Should().BeFalse();
        _usuarioRepositoryMock.Verify(r => r.UpdateAsync(usuario, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConUsuarioInexistente_DevuelveNull()
    {
        var id = Guid.NewGuid();
        _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(new UsuarioId(id), It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        var resultado = await _sut.Handle(new CambiarActivoUsuarioCommand(id, false), CancellationToken.None);

        resultado.Should().BeNull();
    }
}
