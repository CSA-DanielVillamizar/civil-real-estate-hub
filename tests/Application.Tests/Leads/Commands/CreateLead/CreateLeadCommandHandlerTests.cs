using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Leads.Commands.CreateLead;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Leads.Services;
using Xunit;

namespace Plataforma.Application.Tests.Leads.Commands.CreateLead;

public sealed class CreateLeadCommandHandlerTests
{
    private readonly Mock<ILeadRepository> _leadRepositoryMock = new();
    // CalculadoraDeObraService es un domain service puro (sin I/O, determinista)
    // — se usa una instancia real en vez de un mock, no aporta nada simularlo.
    private readonly CreateLeadCommandHandler _sut;

    public CreateLeadCommandHandlerTests()
    {
        _sut = new CreateLeadCommandHandler(_leadRepositoryMock.Object, new CalculadoraDeObraService());
    }

    [Fact]
    public async Task Handle_ConDatosValidos_LlamaAlRepositorioConUnLeadQueRepresentaFielmenteElComando()
    {
        var command = new CreateLeadCommand(
            "Ana Restrepo",
            "ana@example.com",
            "3109876543",
            "+57",
            OrigenLead.FormularioContacto,
            PropiedadDeInteresId: null,
            DatosCalculoObra: null,
            ServicioDeInteres: null,
            Mensaje: null);

        await _sut.Handle(command, CancellationToken.None);

        _leadRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<Lead>(lead =>
                lead.Nombre == "Ana Restrepo" &&
                lead.Email.Valor == "ana@example.com" &&
                lead.Telefono.Numero == "3109876543" &&
                lead.Origen == OrigenLead.FormularioContacto &&
                lead.Estado == EstadoLead.Nuevo &&
                lead.ResultadoCalculadora == null),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ConOrigenCalculadoraObra_CalculaLaEstimacionYLaAsignaAlLeadPersistido()
    {
        var datosCalculoObra = new DatosCalculoObraInput(100, TipoAcabado.Basico, "Gómez Plata", TipoProyecto.Vivienda);
        var command = new CreateLeadCommand(
            "Daniel Villamizar",
            "daniel@example.com",
            "3001234567",
            null,
            OrigenLead.CalculadoraObra,
            null,
            datosCalculoObra,
            null,
            null);

        await _sut.Handle(command, CancellationToken.None);

        _leadRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<Lead>(lead =>
                lead.ResultadoCalculadora != null &&
                // Basico, 100 m² → costo base 180.000.000 (ver CalculadoraDeObraServiceTests).
                lead.ResultadoCalculadora.MontoMinimo.Monto == 180_000_000m * TarifarioObra.FactorMinimo &&
                lead.ResultadoCalculadora.MontoMaximo.Monto == 180_000_000m * TarifarioObra.FactorMaximo),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DevuelveElIdYElEstadoDelLeadCreado()
    {
        var command = new CreateLeadCommand(
            "Ana Restrepo", "ana@example.com", "3109876543", null,
            OrigenLead.FormularioContacto, null, null, null, null);

        var resultado = await _sut.Handle(command, CancellationToken.None);

        resultado.Id.Should().NotBeEmpty();
        resultado.Estado.Should().Be(nameof(EstadoLead.Nuevo));
        resultado.EstimacionCosto.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ConServicioDeInteresYMensajeExplicitos_LosAsignaAlLeadPersistido()
    {
        var command = new CreateLeadCommand(
            "Ana Restrepo", "ana@example.com", "3109876543", null,
            OrigenLead.FormularioContacto, null, null,
            ServicioDeInteres.InterventoriaYPresupuestos, "Necesito interventoría para un proyecto en Rionegro.");

        await _sut.Handle(command, CancellationToken.None);

        _leadRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<Lead>(lead =>
                lead.ServicioDeInteres == ServicioDeInteres.InterventoriaYPresupuestos &&
                lead.Mensaje == "Necesito interventoría para un proyecto en Rionegro."),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ConPropiedadDeInteres_AsignaElIdDeLaPropiedadAlLead()
    {
        var propiedadId = Guid.NewGuid();
        var command = new CreateLeadCommand(
            "Ana Restrepo", "ana@example.com", "3109876543", null,
            OrigenLead.FormularioContacto, propiedadId, null, null, null);

        await _sut.Handle(command, CancellationToken.None);

        _leadRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<Lead>(lead => lead.PropiedadDeInteresId!.Value.Value == propiedadId),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
