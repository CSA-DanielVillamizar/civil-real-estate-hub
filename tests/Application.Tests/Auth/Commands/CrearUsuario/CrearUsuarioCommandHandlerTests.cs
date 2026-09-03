using FluentAssertions;
using Moq;
using Plataforma.Application.Auth.Commands.CrearUsuario;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Usuarios;
using Plataforma.Domain.Usuarios.Exceptions;
using Xunit;

namespace Plataforma.Application.Tests.Auth.Commands.CrearUsuario;

public sealed class CrearUsuarioCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly CrearUsuarioCommandHandler _sut;

    public CrearUsuarioCommandHandlerTests()
    {
        _sut = new CrearUsuarioCommandHandler(_usuarioRepositoryMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_ConEmailNuevo_HasheaLaPasswordYPersisteElUsuario()
    {
        _usuarioRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);
        _passwordHasherMock.Setup(h => h.Hash("clave-segura")).Returns("hash-resultante");

        var resultado = await _sut.Handle(
            new CrearUsuarioCommand("Laura Gómez", "laura@example.com", "clave-segura", RolUsuario.AsesorComercial), CancellationToken.None);

        resultado.Nombre.Should().Be("Laura Gómez");
        resultado.Rol.Should().Be(nameof(RolUsuario.AsesorComercial));
        _usuarioRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Usuario>(u => u.PasswordHash == "hash-resultante"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ConEmailYaRegistrado_LanzaEmailYaRegistradoExceptionSinPersistir()
    {
        var existente = Usuario.Crear("Otro", Email.Crear("laura@example.com"), "hash", RolUsuario.Admin);
        _usuarioRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())).ReturnsAsync(existente);

        var accion = () => _sut.Handle(
            new CrearUsuarioCommand("Laura Gómez", "laura@example.com", "clave-segura", RolUsuario.AsesorComercial), CancellationToken.None);

        await accion.Should().ThrowAsync<EmailYaRegistradoException>();
        _usuarioRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
