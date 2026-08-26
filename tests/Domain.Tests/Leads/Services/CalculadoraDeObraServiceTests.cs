using FluentAssertions;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Leads.Services;
using Plataforma.Domain.Leads.ValueObjects;
using Xunit;

namespace Plataforma.Domain.Tests.Leads.Services;

public sealed class CalculadoraDeObraServiceTests
{
    private readonly CalculadoraDeObraService _sut = new();

    [Fact]
    public void Calcular_ConDatosValidos_AplicaLosFactoresMinimoYMaximoDeTarifarioObra()
    {
        // Basico: 1.800.000 COP/m² × 100 m² = 180.000.000 costo base.
        var datos = DatosCalculoObra.Crear(100, TipoAcabado.Basico, "Gómez Plata", TipoProyecto.Vivienda);

        var estimacion = _sut.Calcular(datos);

        estimacion.MontoMinimo.Monto.Should().Be(180_000_000m * TarifarioObra.FactorMinimo);
        estimacion.MontoMaximo.Monto.Should().Be(180_000_000m * TarifarioObra.FactorMaximo);
        estimacion.MontoMinimo.Moneda.Should().Be("COP");
        estimacion.MontoMaximo.Moneda.Should().Be("COP");
    }

    [Theory]
    [InlineData(TipoAcabado.Basico, 1_800_000)]
    [InlineData(TipoAcabado.Medio, 2_600_000)]
    [InlineData(TipoAcabado.Alto, 3_800_000)]
    public void Calcular_UsaElCostoBasePorM2QueCorrespondeAlTipoDeAcabado(TipoAcabado tipoAcabado, decimal costoBasePorM2Esperado)
    {
        var datos = DatosCalculoObra.Crear(100, tipoAcabado, "Gómez Plata", TipoProyecto.Vivienda);

        var estimacion = _sut.Calcular(datos);

        var costoBaseTotalEsperado = costoBasePorM2Esperado * 100;
        estimacion.MontoMinimo.Monto.Should().Be(costoBaseTotalEsperado * TarifarioObra.FactorMinimo);
    }

    [Fact]
    public void Calcular_ElDesgloseSumaExactamenteElCostoBaseTotal()
    {
        var datos = DatosCalculoObra.Crear(100, TipoAcabado.Basico, "Gómez Plata", TipoProyecto.Vivienda);
        const decimal costoBaseTotalEsperado = 180_000_000m;

        var estimacion = _sut.Calcular(datos);

        estimacion.Desglose.Sum(item => item.Monto.Monto).Should().Be(costoBaseTotalEsperado);
    }

    [Fact]
    public void Calcular_ElDesgloseContieneLasCuatroCategoriasConSusMontosExactos()
    {
        var datos = DatosCalculoObra.Crear(100, TipoAcabado.Basico, "Gómez Plata", TipoProyecto.Vivienda);

        var estimacion = _sut.Calcular(datos);

        estimacion.Desglose.Should().BeEquivalentTo(new[]
        {
            new { Categoria = "ManoDeObra", Monto = new { Monto = 63_000_000m } },
            new { Categoria = "Materiales", Monto = new { Monto = 81_000_000m } },
            new { Categoria = "Equipos", Monto = new { Monto = 18_000_000m } },
            new { Categoria = "AdministracionYUtilidad", Monto = new { Monto = 18_000_000m } },
        });
    }

    [Fact]
    public void Calcular_ConservaLosDatosDeEntradaEnLaEstimacion()
    {
        var datos = DatosCalculoObra.Crear(85.5m, TipoAcabado.Alto, "Medellín", TipoProyecto.Comercial);

        var estimacion = _sut.Calcular(datos);

        estimacion.DatosEntrada.Should().Be(datos);
    }

    [Fact]
    public void Calcular_ConDatosNulos_LanzaArgumentNullException()
    {
        var act = () => _sut.Calcular(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(100_000.01)]
    public void DatosCalculoObra_Crear_ConAreaFueraDeRango_LanzaArgumentException(decimal areaInvalida)
    {
        var act = () => DatosCalculoObra.Crear(areaInvalida, TipoAcabado.Medio, "Gómez Plata", TipoProyecto.Vivienda);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void DatosCalculoObra_Crear_ConMunicipioVacio_LanzaArgumentException(string municipioInvalido)
    {
        var act = () => DatosCalculoObra.Crear(100, TipoAcabado.Medio, municipioInvalido, TipoProyecto.Vivienda);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DatosCalculoObra_Crear_ConDatosValidos_RecortaEspaciosEnElMunicipio()
    {
        var datos = DatosCalculoObra.Crear(100, TipoAcabado.Medio, "  Gómez Plata  ", TipoProyecto.Vivienda);

        datos.Municipio.Should().Be("Gómez Plata");
    }
}
