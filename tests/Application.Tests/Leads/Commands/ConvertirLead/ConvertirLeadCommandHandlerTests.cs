using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Leads.Commands.ConvertirLead;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Leads.Exceptions;
using Plataforma.Domain.Leads.ValueObjects;
using Xunit;

namespace Plataforma.Application.Tests.Leads.Commands.ConvertirLead;

public sealed class ConvertirLeadCommandHandlerTests
{
    private readonly Mock<ILeadRepository> _leadRepositoryMock = new();
    private readonly ConvertirLeadCommandHandler _sut;

    public ConvertirLeadCommandHandlerTests()
    {
        _sut = new ConvertirLeadCommandHandler(_leadRepositoryMock.Object);
    }

    private static Lead CrearLeadCalificado()
    {
        var lead = Lead.Registrar("Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"), OrigenLead.FormularioContacto);
        lead.MarcarContactado();
        lead.Calificar();
        return lead;
    }

    [Fact]
    public async Task Handle_ConLeadCalificado_LoConvierteYPersiste()
    {
        var lead = CrearLeadCalificado();
        _leadRepositoryMock.Setup(r => r.GetByIdAsync(lead.Id, It.IsAny<CancellationToken>())).ReturnsAsync(lead);

        var resultado = await _sut.Handle(new ConvertirLeadCommand(lead.Id.Value), CancellationToken.None);

        resultado!.Estado.Should().Be(nameof(EstadoLead.Convertido));
        _leadRepositoryMock.Verify(r => r.UpdateAsync(lead, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConLeadInexistente_DevuelveNull()
    {
        var leadId = Guid.NewGuid();
        _leadRepositoryMock.Setup(r => r.GetByIdAsync(new LeadId(leadId), It.IsAny<CancellationToken>())).ReturnsAsync((Lead?)null);

        var resultado = await _sut.Handle(new ConvertirLeadCommand(leadId), CancellationToken.None);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ConLeadNuevo_LanzaEstadoLeadInvalidoException()
    {
        var lead = Lead.Registrar("Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"), OrigenLead.FormularioContacto);
        _leadRepositoryMock.Setup(r => r.GetByIdAsync(lead.Id, It.IsAny<CancellationToken>())).ReturnsAsync(lead);

        var act = () => _sut.Handle(new ConvertirLeadCommand(lead.Id.Value), CancellationToken.None);

        await act.Should().ThrowAsync<EstadoLeadInvalidoException>();
    }
}
