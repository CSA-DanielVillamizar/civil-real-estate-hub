using FluentAssertions;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Leads.Events;
using Plataforma.Domain.Leads.Exceptions;
using Plataforma.Domain.Leads.Services;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Propiedades;
using Xunit;

namespace Plataforma.Domain.Tests.Leads;

public sealed class LeadTests
{
    private static Lead CrearLeadNuevo() => Lead.Registrar(
        "Ana Restrepo",
        Email.Crear("ana@example.com"),
        Telefono.Crear("3109876543"),
        OrigenLead.FormularioContacto);

    [Fact]
    public void Registrar_ConOrigenCalculadoraObra_InfiereServicioDeInteresCalculadoraDeObra()
    {
        var datos = DatosCalculoObra.Crear(100, TipoAcabado.Basico, "Gómez Plata", TipoProyecto.Vivienda);
        var estimacion = new CalculadoraDeObraService().Calcular(datos);

        var lead = Lead.Registrar(
            "Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"),
            OrigenLead.CalculadoraObra, resultadoCalculadora: estimacion);

        lead.ServicioDeInteres.Should().Be(ServicioDeInteres.CalculadoraDeObra);
    }

    [Fact]
    public void Registrar_ConPropiedadDeInteres_InfiereServicioDeInteresInmobiliaria()
    {
        var lead = Lead.Registrar(
            "Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"),
            OrigenLead.FormularioContacto, propiedadDeInteresId: PropiedadId.Nueva());

        lead.ServicioDeInteres.Should().Be(ServicioDeInteres.Inmobiliaria);
    }

    [Fact]
    public void Registrar_SinSenalDelDominioNiServicioExplicito_DejaServicioDeInteresEnNull()
    {
        var lead = CrearLeadNuevo();

        lead.ServicioDeInteres.Should().BeNull();
    }

    [Fact]
    public void Registrar_ConServicioDeInteresExplicito_LoRespetaEnVezDeInferir()
    {
        var lead = Lead.Registrar(
            "Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"),
            OrigenLead.FormularioContacto, servicioDeInteres: ServicioDeInteres.InterventoriaYPresupuestos);

        lead.ServicioDeInteres.Should().Be(ServicioDeInteres.InterventoriaYPresupuestos);
    }

    [Fact]
    public void Registrar_ConMensajeConEspaciosAlrededor_LoRecortaYLoGuarda()
    {
        var lead = Lead.Registrar(
            "Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"),
            OrigenLead.FormularioContacto, mensaje: "  Tengo un lote de 800m² en Rionegro.  ");

        lead.Mensaje.Should().Be("Tengo un lote de 800m² en Rionegro.");
    }

    [Fact]
    public void Registrar_ConMensajeVacio_LoDejaEnNull()
    {
        var lead = Lead.Registrar(
            "Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"),
            OrigenLead.FormularioContacto, mensaje: "   ");

        lead.Mensaje.Should().BeNull();
    }

    [Fact]
    public void CalificarPorDescargaDePdf_ConLeadNuevo_TransicionaACalificadoYDisparaEvento()
    {
        var lead = CrearLeadNuevo();
        lead.ClearDomainEvents();

        lead.CalificarPorDescargaDePdf();

        lead.Estado.Should().Be(EstadoLead.Calificado);
        lead.DomainEvents.Should().ContainSingle(e => e is LeadCalificadoEvent);
    }

    [Fact]
    public void CalificarPorDescargaDePdf_ConLeadYaCalificado_EsIdempotenteYNoDisparaOtroEvento()
    {
        var lead = CrearLeadNuevo();
        lead.CalificarPorDescargaDePdf();
        lead.ClearDomainEvents();

        lead.CalificarPorDescargaDePdf();

        lead.Estado.Should().Be(EstadoLead.Calificado);
        lead.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void CalificarPorDescargaDePdf_ConLeadDescartado_LanzaEstadoLeadInvalidoException()
    {
        var lead = CrearLeadNuevo();
        lead.Descartar("No responde.");

        var act = lead.CalificarPorDescargaDePdf;

        act.Should().Throw<EstadoLeadInvalidoException>();
    }

    [Fact]
    public void CalificarPorDescargaDePdf_ConLeadConvertido_LanzaEstadoLeadInvalidoException()
    {
        var lead = CrearLeadNuevo();
        lead.MarcarContactado();
        lead.Calificar();
        lead.Convertir();

        var act = lead.CalificarPorDescargaDePdf;

        act.Should().Throw<EstadoLeadInvalidoException>();
    }

    [Fact]
    public void MarcarNotificacionComercialEnviada_ConLeadSinNotificar_RegistraLaMarcaDeTiempo()
    {
        var lead = CrearLeadNuevo();

        lead.MarcarNotificacionComercialEnviada();

        lead.NotificacionComercialEnviadaEn.Should().NotBeNull();
        lead.NotificacionComercialEnviadaEn.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void MarcarNotificacionComercialEnviada_LlamadoDosVeces_EsIdempotenteYConservaLaPrimeraMarca()
    {
        var lead = CrearLeadNuevo();
        lead.MarcarNotificacionComercialEnviada();
        var primeraMarca = lead.NotificacionComercialEnviadaEn;

        lead.MarcarNotificacionComercialEnviada();

        lead.NotificacionComercialEnviadaEn.Should().Be(primeraMarca);
    }

    [Fact]
    public void MarcarNotificacionComercialEnviada_NoDisparaEventosDeDominio()
    {
        var lead = CrearLeadNuevo();
        lead.ClearDomainEvents();

        lead.MarcarNotificacionComercialEnviada();

        lead.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Registrar_AsignaCapturadoEnAlMomentoDeLaCreacion()
    {
        var lead = CrearLeadNuevo();

        lead.CapturadoEn.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void MarcarContactado_ConLeadNuevo_TransicionaAContactado()
    {
        var lead = CrearLeadNuevo();

        lead.MarcarContactado();

        lead.Estado.Should().Be(EstadoLead.Contactado);
    }

    [Fact]
    public void MarcarContactado_ConLeadYaContactado_LanzaEstadoLeadInvalidoException()
    {
        var lead = CrearLeadNuevo();
        lead.MarcarContactado();

        var act = lead.MarcarContactado;

        act.Should().Throw<EstadoLeadInvalidoException>();
    }

    [Fact]
    public void Calificar_ConLeadContactado_TransicionaACalificadoYDisparaEvento()
    {
        var lead = CrearLeadNuevo();
        lead.MarcarContactado();
        lead.ClearDomainEvents();

        lead.Calificar();

        lead.Estado.Should().Be(EstadoLead.Calificado);
        lead.DomainEvents.Should().ContainSingle(e => e is LeadCalificadoEvent);
    }

    [Fact]
    public void Calificar_ConLeadNuevo_LanzaEstadoLeadInvalidoException()
    {
        var lead = CrearLeadNuevo();

        var act = lead.Calificar;

        act.Should().Throw<EstadoLeadInvalidoException>();
    }

    [Fact]
    public void Convertir_ConLeadCalificado_TransicionaAConvertidoYDisparaEvento()
    {
        var lead = CrearLeadNuevo();
        lead.MarcarContactado();
        lead.Calificar();
        lead.ClearDomainEvents();

        lead.Convertir();

        lead.Estado.Should().Be(EstadoLead.Convertido);
        lead.DomainEvents.Should().ContainSingle(e => e is LeadConvertidoEvent);
    }

    [Fact]
    public void Convertir_ConLeadNuevo_LanzaEstadoLeadInvalidoException()
    {
        var lead = CrearLeadNuevo();

        var act = lead.Convertir;

        act.Should().Throw<EstadoLeadInvalidoException>();
    }

    [Fact]
    public void Descartar_ConLeadNuevo_TransicionaADescartadoYDisparaEvento()
    {
        var lead = CrearLeadNuevo();
        lead.ClearDomainEvents();

        lead.Descartar("No contesta el teléfono.");

        lead.Estado.Should().Be(EstadoLead.Descartado);
        lead.DomainEvents.Should().ContainSingle(e => e is LeadDescartadoEvent);
    }

    [Fact]
    public void Descartar_ConLeadYaConvertido_LanzaEstadoLeadInvalidoException()
    {
        var lead = CrearLeadNuevo();
        lead.MarcarContactado();
        lead.Calificar();
        lead.Convertir();

        var act = () => lead.Descartar("Cambió de opinión.");

        act.Should().Throw<EstadoLeadInvalidoException>();
    }
}
