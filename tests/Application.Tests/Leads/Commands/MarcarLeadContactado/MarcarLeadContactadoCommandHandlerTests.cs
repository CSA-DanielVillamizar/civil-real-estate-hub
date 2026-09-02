using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Leads.Commands.MarcarLeadContactado;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Leads.Exceptions;
using Plataforma.Domain.Leads.ValueObjects;
using Xunit;

namespace Plataforma.Application.Tests.Leads.Commands.MarcarLeadContactado;

public sealed class MarcarLeadContactadoCommandHandlerTests
{
    private readonly Mock<ILeadRepository> _leadRepositoryMock = new();
    private readonly MarcarLeadContactadoCommandHandler _sut;

    public MarcarLeadContactadoCommandHandlerTests()
    {
        _sut = new MarcarLeadContactadoCommandHandler(_leadRepositoryMock.Object);
    }

    private static Lead CrearLeadNuevo() => Lead.Registrar(
        "Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"), OrigenLead.FormularioContacto);

    [Fact]
    public async Task Handle_ConLeadNuevo_LoMarcaContactadoYPersiste()
    {
        var lead = CrearLeadNuevo();
        _leadRepositoryMock.Setup(r => r.GetByIdAsync(lead.Id, It.IsAny<CancellationToken>())).ReturnsAsync(lead);

        var resultado = await _sut.Handle(new MarcarLeadContactadoCommand(lead.Id.Value), CancellationToken.None);

        resultado!.Estado.Should().Be(nameof(EstadoLead.Contactado));
        _leadRepositoryMock.Verify(r => r.UpdateAsync(lead, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConLeadInexistente_DevuelveNull()
    {
        var leadId = Guid.NewGuid();
        _leadRepositoryMock.Setup(r => r.GetByIdAsync(new LeadId(leadId), It.IsAny<CancellationToken>())).ReturnsAsync((Lead?)null);

        var resultado = await _sut.Handle(new MarcarLeadContactadoCommand(leadId), CancellationToken.None);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ConLeadYaContactado_LanzaEstadoLeadInvalidoException()
    {
        var lead = CrearLeadNuevo();
        lead.MarcarContactado();
        _leadRepositoryMock.Setup(r => r.GetByIdAsync(lead.Id, It.IsAny<CancellationToken>())).ReturnsAsync(lead);

        var act = () => _sut.Handle(new MarcarLeadContactadoCommand(lead.Id.Value), CancellationToken.None);

        await act.Should().ThrowAsync<EstadoLeadInvalidoException>();
    }
}
