using FluentAssertions;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Usuarios;
using Xunit;

namespace Plataforma.Domain.Tests.Usuarios;

public sealed class UsuarioTests
{
    [Fact]
    public void Crear_ConDatosValidos_InicializaActivoYCreadoEn()
    {
        var usuario = Usuario.Crear("Daniel Villamizar", Email.Crear("daniel@example.com"), "hash-simulado", RolUsuario.Admin);

        usuario.Nombre.Should().Be("Daniel Villamizar");
        usuario.Email.Valor.Should().Be("daniel@example.com");
        usuario.PasswordHash.Should().Be("hash-simulado");
        usuario.Rol.Should().Be(RolUsuario.Admin);
        usuario.Activo.Should().BeTrue();
        usuario.CreadoEn.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Crear_ConNombreVacio_LanzaArgumentException(string nombreInvalido)
    {
        var accion = () => Usuario.Crear(nombreInvalido, Email.Crear("daniel@example.com"), "hash", RolUsuario.Admin);

        accion.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Crear_ConPasswordHashVacio_LanzaArgumentException(string hashInvalido)
    {
        var accion = () => Usuario.Crear("Daniel Villamizar", Email.Crear("daniel@example.com"), hashInvalido, RolUsuario.Admin);

        accion.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ActualizarPasswordHash_ConHashValido_ReemplazaElHashExistente()
    {
        var usuario = Usuario.Crear("Daniel Villamizar", Email.Crear("daniel@example.com"), "hash-viejo", RolUsuario.AsesorComercial);

        usuario.ActualizarPasswordHash("hash-nuevo");

        usuario.PasswordHash.Should().Be("hash-nuevo");
    }

    [Fact]
    public void ActualizarPasswordHash_ConHashVacio_LanzaArgumentException()
    {
        var usuario = Usuario.Crear("Daniel Villamizar", Email.Crear("daniel@example.com"), "hash-viejo", RolUsuario.AsesorComercial);

        var accion = () => usuario.ActualizarPasswordHash("  ");

        accion.Should().Throw<ArgumentException>();
    }
}
