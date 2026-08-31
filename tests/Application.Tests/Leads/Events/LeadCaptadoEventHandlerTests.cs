using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Common.Messaging;
using Plataforma.Application.Leads.Events;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Leads.Events;
using Xunit;

namespace Plataforma.Application.Tests.Leads.Events;

public sealed class LeadCaptadoEventHandlerTests
{
    private readonly Mock<ILeadNotificationQueue> _queueMock = new();
    private readonly Mock<ILogger<LeadCaptadoEventHandler>> _loggerMock = new();
    private readonly LeadCaptadoEventHandler _sut;

    public LeadCaptadoEventHandlerTests()
    {
        _sut = new LeadCaptadoEventHandler(_queueMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_EncolaUnMensajeConElIdDelLead()
    {
        var leadId = LeadId.Nueva();
        var evento = new LeadCaptadoEvent(leadId, OrigenLead.CalculadoraObra);

        await _sut.Handle(evento, CancellationToken.None);

        _queueMock.Verify(q => q.EncolarAsync(
            It.Is<LeadCaptadoNotificationMessage>(m => m.LeadId == leadId.Value),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // Este es el caso crítico: un fallo en el encolado (Storage Queue caído,
    // credenciales, etc.) NO debe propagarse — el handler corre dentro de la
    // misma transacción que persiste el Lead, y si esta excepción escapara,
    // revertiría la captación del lead por un problema del sistema de
    // notificaciones, exactamente lo que Fase 2 pidió evitar.
    [Fact]
    public async Task Handle_ConElEncoladoFallando_NoPropagaLaExcepcion()
    {
        var evento = new LeadCaptadoEvent(LeadId.Nueva(), OrigenLead.CalculadoraObra);
        _queueMock
            .Setup(q => q.EncolarAsync(It.IsAny<LeadCaptadoNotificationMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("La cola no está disponible."));

        var act = () => _sut.Handle(evento, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }
}
