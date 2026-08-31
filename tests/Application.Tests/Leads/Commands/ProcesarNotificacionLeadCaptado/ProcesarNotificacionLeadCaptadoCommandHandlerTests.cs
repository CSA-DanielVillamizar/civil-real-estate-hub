using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Leads.Commands.ProcesarNotificacionLeadCaptado;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Leads.ValueObjects;
using Xunit;

namespace Plataforma.Application.Tests.Leads.Commands.ProcesarNotificacionLeadCaptado;

public sealed class ProcesarNotificacionLeadCaptadoCommandHandlerTests
{
    private readonly Mock<ILeadRepository> _leadRepositoryMock = new();
    private readonly Mock<INotificacionComercialService> _notificacionComercialMock = new();
    private readonly Mock<IEmailBienvenidaService> _emailBienvenidaMock = new();
    private readonly ProcesarNotificacionLeadCaptadoCommandHandler _sut;

    public ProcesarNotificacionLeadCaptadoCommandHandlerTests()
    {
        _sut = new ProcesarNotificacionLeadCaptadoCommandHandler(
            _leadRepositoryMock.Object,
            _notificacionComercialMock.Object,
            _emailBienvenidaMock.Object);
    }

    private static Lead CrearLeadNuevo() => Lead.Registrar(
        "Ana Restrepo",
        Email.Crear("ana@example.com"),
        Telefono.Crear("3109876543"),
        OrigenLead.FormularioContacto);

    [Fact]
    public async Task Handle_ConLeadNoNotificado_LlamaAlWebhookYAlCorreoYMarcaLaNotificacion()
    {
        var lead = CrearLeadNuevo();
        _leadRepositoryMock.Setup(r => r.GetByIdAsync(lead.Id, It.IsAny<CancellationToken>())).ReturnsAsync(lead);

        await _sut.Handle(new ProcesarNotificacionLeadCaptadoCommand(lead.Id.Value), CancellationToken.None);

        _notificacionComercialMock.Verify(s => s.NotificarNuevoLeadAsync(lead, It.IsAny<CancellationToken>()), Times.Once);
        _emailBienvenidaMock.Verify(s => s.EnviarBienvenidaAsync(lead, It.IsAny<CancellationToken>()), Times.Once);
        lead.NotificacionComercialEnviadaEn.Should().NotBeNull();
        _leadRepositoryMock.Verify(r => r.UpdateAsync(lead, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConLeadYaNotificado_NoLlamaANingunServicioNiPersisteDeNuevo()
    {
        var lead = CrearLeadNuevo();
        lead.MarcarNotificacionComercialEnviada();
        _leadRepositoryMock.Setup(r => r.GetByIdAsync(lead.Id, It.IsAny<CancellationToken>())).ReturnsAsync(lead);

        await _sut.Handle(new ProcesarNotificacionLeadCaptadoCommand(lead.Id.Value), CancellationToken.None);

        _notificacionComercialMock.Verify(s => s.NotificarNuevoLeadAsync(It.IsAny<Lead>(), It.IsAny<CancellationToken>()), Times.Never);
        _emailBienvenidaMock.Verify(s => s.EnviarBienvenidaAsync(It.IsAny<Lead>(), It.IsAny<CancellationToken>()), Times.Never);
        _leadRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Lead>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ConLeadInexistente_LanzaInvalidOperationException()
    {
        var leadId = LeadId.Nueva();
        _leadRepositoryMock.Setup(r => r.GetByIdAsync(leadId, It.IsAny<CancellationToken>())).ReturnsAsync((Lead?)null);

        var act = () => _sut.Handle(new ProcesarNotificacionLeadCaptadoCommand(leadId.Value), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_ConElWebhookFallando_NoEnviaElCorreoNiMarcaLaNotificacion()
    {
        var lead = CrearLeadNuevo();
        _leadRepositoryMock.Setup(r => r.GetByIdAsync(lead.Id, It.IsAny<CancellationToken>())).ReturnsAsync(lead);
        _notificacionComercialMock
            .Setup(s => s.NotificarNuevoLeadAsync(lead, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("El webhook no respondió."));

        var act = () => _sut.Handle(new ProcesarNotificacionLeadCaptadoCommand(lead.Id.Value), CancellationToken.None);

        // "Todo o nada" (ver el handler): si el webhook falla, ni el correo
        // se envía ni queda marcado como notificado — el mensaje completo de
        // la cola se reintenta.
        await act.Should().ThrowAsync<HttpRequestException>();
        _emailBienvenidaMock.Verify(s => s.EnviarBienvenidaAsync(It.IsAny<Lead>(), It.IsAny<CancellationToken>()), Times.Never);
        lead.NotificacionComercialEnviadaEn.Should().BeNull();
        _leadRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Lead>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
