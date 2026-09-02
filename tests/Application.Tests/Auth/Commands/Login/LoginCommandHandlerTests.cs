using FluentAssertions;
using Moq;
using Plataforma.Application.Auth.Commands.Login;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Usuarios;
using Xunit;

namespace Plataforma.Application.Tests.Auth.Commands.Login;

public sealed class LoginCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new();
    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTests()
    {
        _sut = new LoginCommandHandler(_usuarioRepositoryMock.Object, _passwordHasherMock.Object, _jwtTokenGeneratorMock.Object);
    }

    private static Usuario CrearUsuario(RolUsuario rol = RolUsuario.Admin) =>
        Usuario.Crear("Daniel Villamizar", Email.Crear("daniel@example.com"), "hash-almacenado", rol);

    [Fact]
    public async Task Handle_ConCredencialesValidas_DevuelveElToken()
    {
        var usuario = CrearUsuario();
        var expiraEn = DateTimeOffset.UtcNow.AddHours(8);
        _usuarioRepositoryMock.Setup(r => r.GetByEmailAsync(It.Is<Email>(e => e.Valor == "daniel@example.com"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        var requiereRehash = false;
        _passwordHasherMock
            .Setup(h => h.Verificar("hash-almacenado", "clave-correcta", out requiereRehash))
            .Returns(true);
        _jwtTokenGeneratorMock.Setup(j => j.Generar(usuario)).Returns(new JwtTokenResult("token-jwt", expiraEn));

        var resultado = await _sut.Handle(new LoginCommand("daniel@example.com", "clave-correcta"), CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado!.Token.Should().Be("token-jwt");
        resultado.Nombre.Should().Be("Daniel Villamizar");
        resultado.Rol.Should().Be(nameof(RolUsuario.Admin));
        _usuarioRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ConPasswordIncorrecta_DevuelveNull()
    {
        var usuario = CrearUsuario();
        _usuarioRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        var requiereRehash = false;
        _passwordHasherMock
            .Setup(h => h.Verificar("hash-almacenado", "clave-incorrecta", out requiereRehash))
            .Returns(false);

        var resultado = await _sut.Handle(new LoginCommand("daniel@example.com", "clave-incorrecta"), CancellationToken.None);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ConEmailInexistente_DevuelveNull()
    {
        _usuarioRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        var resultado = await _sut.Handle(new LoginCommand("nadie@example.com", "cualquiera"), CancellationToken.None);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ConUsuarioInactivo_DevuelveNull()
    {
        var usuario = CrearUsuario();
        typeof(Usuario).GetProperty(nameof(Usuario.Activo))!.SetValue(usuario, false);
        _usuarioRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())).ReturnsAsync(usuario);

        var resultado = await _sut.Handle(new LoginCommand("daniel@example.com", "clave-correcta"), CancellationToken.None);

        resultado.Should().BeNull();
        _passwordHasherMock.Verify(
            h => h.Verificar(It.IsAny<string>(), It.IsAny<string>(), out It.Ref<bool>.IsAny),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ConEmailConFormatoInvalido_DevuelveNullSinConsultarElRepositorio()
    {
        var resultado = await _sut.Handle(new LoginCommand("no-es-un-email", "clave"), CancellationToken.None);

        resultado.Should().BeNull();
        _usuarioRepositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ConHashDesactualizado_LoRegeneraYPersiste()
    {
        var usuario = CrearUsuario();
        _usuarioRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        var requiereRehash = true;
        _passwordHasherMock
            .Setup(h => h.Verificar("hash-almacenado", "clave-correcta", out requiereRehash))
            .Returns(true);
        _passwordHasherMock.Setup(h => h.Hash("clave-correcta")).Returns("hash-nuevo");
        _jwtTokenGeneratorMock.Setup(j => j.Generar(usuario)).Returns(new JwtTokenResult("token-jwt", DateTimeOffset.UtcNow.AddHours(8)));

        await _sut.Handle(new LoginCommand("daniel@example.com", "clave-correcta"), CancellationToken.None);

        usuario.PasswordHash.Should().Be("hash-nuevo");
        _usuarioRepositoryMock.Verify(r => r.UpdateAsync(usuario, It.IsAny<CancellationToken>()), Times.Once);
    }
}
