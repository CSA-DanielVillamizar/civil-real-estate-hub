using FluentAssertions;
using Plataforma.Domain.Confianza;
using Plataforma.Domain.Leads;
using Xunit;

namespace Plataforma.Domain.Tests.Confianza;

public sealed class ContenidoConfianzaTests
{
    [Fact]
    public void Crear_ConDatosValidos_InicializaSinPublicarYConCreadoEn()
    {
        var contenido = ContenidoConfianza.Crear(
            TipoContenidoConfianza.Testimonio, "Ana Restrepo", "Excelente trabajo.", "Rionegro", ServicioDeInteres.ConsultoriaYDisenoEstructural);

        contenido.Tipo.Should().Be(TipoContenidoConfianza.Testimonio);
        contenido.Titulo.Should().Be("Ana Restrepo");
        contenido.Descripcion.Should().Be("Excelente trabajo.");
        contenido.Municipio.Should().Be("Rionegro");
        contenido.ServicioRelacionado.Should().Be(ServicioDeInteres.ConsultoriaYDisenoEstructural);
        contenido.Publicado.Should().BeFalse();
        contenido.CreadoEn.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Crear_SinMunicipio_LoDejaNulo()
    {
        var contenido = ContenidoConfianza.Crear(
            TipoContenidoConfianza.Portafolio, "Proyecto X", "Descripción del caso.", null, ServicioDeInteres.InterventoriaYPresupuestos);

        contenido.Municipio.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Crear_ConTituloVacio_LanzaArgumentException(string tituloInvalido)
    {
        var accion = () => ContenidoConfianza.Crear(
            TipoContenidoConfianza.Testimonio, tituloInvalido, "Descripción.", null, ServicioDeInteres.Inmobiliaria);

        accion.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Crear_ConDescripcionVacia_LanzaArgumentException(string descripcionInvalida)
    {
        var accion = () => ContenidoConfianza.Crear(
            TipoContenidoConfianza.Testimonio, "Título", descripcionInvalida, null, ServicioDeInteres.Inmobiliaria);

        accion.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Publicar_DejaElContenidoPublicado()
    {
        var contenido = ContenidoConfianza.Crear(
            TipoContenidoConfianza.Testimonio, "Ana Restrepo", "Excelente.", null, ServicioDeInteres.Inmobiliaria);

        contenido.Publicar();

        contenido.Publicado.Should().BeTrue();
    }

    [Fact]
    public void Despublicar_DejaElContenidoSinPublicar()
    {
        var contenido = ContenidoConfianza.Crear(
            TipoContenidoConfianza.Testimonio, "Ana Restrepo", "Excelente.", null, ServicioDeInteres.Inmobiliaria);
        contenido.Publicar();

        contenido.Despublicar();

        contenido.Publicado.Should().BeFalse();
    }

    [Fact]
    public void Actualizar_ConDatosValidos_ReemplazaLosCampos()
    {
        var contenido = ContenidoConfianza.Crear(
            TipoContenidoConfianza.Portafolio, "Proyecto viejo", "Descripción vieja.", "La Ceja", ServicioDeInteres.Inmobiliaria);

        contenido.Actualizar("Proyecto nuevo", "Descripción nueva.", "Guarne", ServicioDeInteres.InterventoriaYPresupuestos);

        contenido.Titulo.Should().Be("Proyecto nuevo");
        contenido.Descripcion.Should().Be("Descripción nueva.");
        contenido.Municipio.Should().Be("Guarne");
        contenido.ServicioRelacionado.Should().Be(ServicioDeInteres.InterventoriaYPresupuestos);
    }
}
