using FluentAssertions;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Tarifas;
using Xunit;

namespace Plataforma.Domain.Tests.Tarifas;

public sealed class PaqueteTarifaTests
{
    [Fact]
    public void Crear_ConDatosValidos_InicializaSinPublicarYConCreadoEn()
    {
        var paquete = PaqueteTarifa.Crear(
            ServicioDeInteres.ConsultoriaYDisenoEstructural, "Diseño estructural residencial", "Incluye planos y memoria de cálculo.",
            50_000, 80_000, "por m²", "COP");

        paquete.ServicioRelacionado.Should().Be(ServicioDeInteres.ConsultoriaYDisenoEstructural);
        paquete.Titulo.Should().Be("Diseño estructural residencial");
        paquete.PrecioDesde.Should().Be(50_000);
        paquete.PrecioHasta.Should().Be(80_000);
        paquete.UnidadPrecio.Should().Be("por m²");
        paquete.Moneda.Should().Be("COP");
        paquete.Publicado.Should().BeFalse();
        paquete.CreadoEn.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Crear_SinRangoDePrecio_LoDejaNulo()
    {
        var paquete = PaqueteTarifa.Crear(
            ServicioDeInteres.InterventoriaYPresupuestos, "Interventoría integral", "Cotización personalizada.", null, null, "por definir", "COP");

        paquete.PrecioDesde.Should().BeNull();
        paquete.PrecioHasta.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Crear_ConTituloVacio_LanzaArgumentException(string tituloInvalido)
    {
        var accion = () => PaqueteTarifa.Crear(
            ServicioDeInteres.Inmobiliaria, tituloInvalido, "Descripción.", null, null, "tarifa plana", "COP");

        accion.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Crear_ConPrecioDesdeMayorQuePrecioHasta_LanzaArgumentException()
    {
        var accion = () => PaqueteTarifa.Crear(
            ServicioDeInteres.Inmobiliaria, "Título", "Descripción.", 100_000, 50_000, "por m²", "COP");

        accion.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Crear_ConPrecioNegativo_LanzaArgumentException()
    {
        var accion = () => PaqueteTarifa.Crear(
            ServicioDeInteres.Inmobiliaria, "Título", "Descripción.", -1, null, "por m²", "COP");

        accion.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Publicar_DejaElPaquetePublicado()
    {
        var paquete = PaqueteTarifa.Crear(ServicioDeInteres.Inmobiliaria, "Título", "Descripción.", null, null, "tarifa plana", "COP");

        paquete.Publicar();

        paquete.Publicado.Should().BeTrue();
    }

    [Fact]
    public void Despublicar_DejaElPaqueteSinPublicar()
    {
        var paquete = PaqueteTarifa.Crear(ServicioDeInteres.Inmobiliaria, "Título", "Descripción.", null, null, "tarifa plana", "COP");
        paquete.Publicar();

        paquete.Despublicar();

        paquete.Publicado.Should().BeFalse();
    }

    [Fact]
    public void Actualizar_ConDatosValidos_ReemplazaLosCampos()
    {
        var paquete = PaqueteTarifa.Crear(ServicioDeInteres.Inmobiliaria, "Viejo", "Descripción vieja.", null, null, "tarifa plana", "COP");

        paquete.Actualizar("Nuevo", "Descripción nueva.", 10_000, 20_000, "por m²", ServicioDeInteres.InterventoriaYPresupuestos);

        paquete.Titulo.Should().Be("Nuevo");
        paquete.Descripcion.Should().Be("Descripción nueva.");
        paquete.PrecioDesde.Should().Be(10_000);
        paquete.PrecioHasta.Should().Be(20_000);
        paquete.UnidadPrecio.Should().Be("por m²");
        paquete.ServicioRelacionado.Should().Be(ServicioDeInteres.InterventoriaYPresupuestos);
    }
}
