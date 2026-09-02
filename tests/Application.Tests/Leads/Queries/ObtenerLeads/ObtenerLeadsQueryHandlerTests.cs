using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Leads.Queries.ObtenerLeads;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Leads.ValueObjects;
using Xunit;

namespace Plataforma.Application.Tests.Leads.Queries.ObtenerLeads;

public sealed class ObtenerLeadsQueryHandlerTests
{
    private readonly Mock<ILeadRepository> _leadRepositoryMock = new();
    private readonly ObtenerLeadsQueryHandler _sut;

    public ObtenerLeadsQueryHandlerTests()
    {
        _sut = new ObtenerLeadsQueryHandler(_leadRepositoryMock.Object);
    }

    private static Lead CrearLeadNuevo() => Lead.Registrar(
        "Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"), OrigenLead.FormularioContacto);

    [Fact]
    public async Task Handle_MapeaCadaLeadAUnListItemConSusDatos()
    {
        var lead = CrearLeadNuevo();
        _leadRepositoryMock.Setup(r => r.ListAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync([lead]);

        var resultado = await _sut.Handle(new ObtenerLeadsQuery(null), CancellationToken.None);

        resultado.Should().ContainSingle();
        var item = resultado[0];
        item.Id.Should().Be(lead.Id.Value);
        item.Nombre.Should().Be("Ana Restrepo");
        item.Email.Should().Be("ana@example.com");
        item.Estado.Should().Be(EstadoLead.Nuevo);
        item.Origen.Should().Be(OrigenLead.FormularioContacto);
    }

    [Fact]
    public async Task Handle_PasaElFiltroDeEstadoAlRepositorio()
    {
        _leadRepositoryMock
            .Setup(r => r.ListAsync(EstadoLead.Calificado, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        await _sut.Handle(new ObtenerLeadsQuery(EstadoLead.Calificado), CancellationToken.None);

        _leadRepositoryMock.Verify(r => r.ListAsync(EstadoLead.Calificado, It.IsAny<CancellationToken>()), Times.Once);
    }
}
