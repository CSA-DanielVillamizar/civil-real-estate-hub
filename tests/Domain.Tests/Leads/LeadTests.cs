using FluentAssertions;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Leads.Events;
using Plataforma.Domain.Leads.Exceptions;
using Plataforma.Domain.Leads.ValueObjects;
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
}
