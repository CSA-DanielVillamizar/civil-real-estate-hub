using FluentAssertions;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Obras;
using Plataforma.Domain.Obras.Exceptions;
using Plataforma.Domain.Propiedades;
using Xunit;

namespace Plataforma.Domain.Tests.Obras;

public sealed class ProyectoObraTests
{
    private static ProyectoObra CrearProyecto() => ProyectoObra.Crear(
        "Ana Restrepo",
        Email.Crear("ana@example.com"),
        Telefono.Crear("3109876543"),
        "Interventoría casa campestre");

    [Fact]
    public void Crear_ConDatosValidos_InicializaEnPlanificacionConTokenYSinHitos()
    {
        var proyecto = CrearProyecto();

        proyecto.Estado.Should().Be(EstadoProyecto.Planificacion);
        proyecto.Hitos.Should().BeEmpty();
        proyecto.TokenAcceso.Should().NotBeNullOrWhiteSpace();
        proyecto.PropiedadId.Should().BeNull();
    }

    [Fact]
    public void Crear_DosVeces_GeneraTokensDistintos()
    {
        var proyecto1 = CrearProyecto();
        var proyecto2 = CrearProyecto();

        proyecto1.TokenAcceso.Should().NotBe(proyecto2.TokenAcceso);
    }

    [Fact]
    public void Crear_ConPropiedadDeInteres_LaAsigna()
    {
        var propiedadId = PropiedadId.Nueva();

        var proyecto = ProyectoObra.Crear(
            "Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"),
            "Construcción lote", propiedadId: propiedadId);

        proyecto.PropiedadId.Should().Be(propiedadId);
    }

    [Fact]
    public void AgregarHito_AsignaOrdenSecuencialEmpezandoEnCero()
    {
        var proyecto = CrearProyecto();

        var hito1 = proyecto.AgregarHito("Cimentación", null, null);
        var hito2 = proyecto.AgregarHito("Estructura", null, null);

        hito1.Orden.Should().Be(0);
        hito2.Orden.Should().Be(1);
        proyecto.Hitos.Should().HaveCount(2);
    }

    [Fact]
    public void AgregarHito_NuevoHito_EmpiezaEnEstadoPendiente()
    {
        var proyecto = CrearProyecto();

        var hito = proyecto.AgregarHito("Cimentación", "Excavación y fundida", null);

        hito.Estado.Should().Be(EstadoHito.Pendiente);
    }

    [Fact]
    public void CambiarEstadoHito_ACompletado_RegistraFechaCompletado()
    {
        var proyecto = CrearProyecto();
        var hito = proyecto.AgregarHito("Cimentación", null, null);

        proyecto.CambiarEstadoHito(hito.Id, EstadoHito.Completado);

        hito.Estado.Should().Be(EstadoHito.Completado);
        hito.FechaCompletado.Should().NotBeNull();
    }

    [Fact]
    public void CambiarEstadoHito_CompletadoDosVeces_ConservaLaPrimeraFechaCompletado()
    {
        var proyecto = CrearProyecto();
        var hito = proyecto.AgregarHito("Cimentación", null, null);

        proyecto.CambiarEstadoHito(hito.Id, EstadoHito.Completado);
        var primeraFecha = hito.FechaCompletado;

        proyecto.CambiarEstadoHito(hito.Id, EstadoHito.Completado);

        hito.FechaCompletado.Should().Be(primeraFecha);
    }

    [Fact]
    public void CambiarEstadoHito_ConIdInexistente_LanzaHitoNoEncontradoException()
    {
        var proyecto = CrearProyecto();

        var accion = () => proyecto.CambiarEstadoHito(Guid.NewGuid(), EstadoHito.Completado);

        accion.Should().Throw<HitoNoEncontradoException>();
    }

    [Fact]
    public void AgregarEvidenciaAHito_ConUrlValida_LaAsignaAlHito()
    {
        var proyecto = CrearProyecto();
        var hito = proyecto.AgregarHito("Cimentación", null, null);

        proyecto.AgregarEvidenciaAHito(hito.Id, "https://blob.example.com/foto.jpg");

        hito.FotoEvidenciaUrl.Should().Be("https://blob.example.com/foto.jpg");
    }

    [Fact]
    public void CambiarEstado_ActualizaElEstadoGeneralDelProyecto()
    {
        var proyecto = CrearProyecto();

        proyecto.CambiarEstado(EstadoProyecto.EnEjecucion);

        proyecto.Estado.Should().Be(EstadoProyecto.EnEjecucion);
    }
}
