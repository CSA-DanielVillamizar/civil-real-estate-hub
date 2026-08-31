using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Leads.Commands.CreateLead;
using Plataforma.Application.Leads.Commands.GenerarPresupuestoPdf;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Leads.Services;
using Xunit;

namespace Plataforma.Application.Tests.Leads.Commands.GenerarPresupuestoPdf;

public sealed class GenerarPresupuestoPdfCommandHandlerTests
{
    private readonly Mock<ILeadRepository> _leadRepositoryMock = new();
    private readonly Mock<IPresupuestoPdfGenerator> _pdfGeneratorMock = new();
    private readonly GenerarPresupuestoPdfCommandHandler _sut;

    public GenerarPresupuestoPdfCommandHandlerTests()
    {
        // CalculadoraDeObraService: instancia real (domain service puro, sin
        // I/O) — ver la misma justificación en CreateLeadCommandHandlerTests.
        _pdfGeneratorMock
            .Setup(g => g.Generar(It.IsAny<Lead>()))
            .Returns([1, 2, 3]);

        _sut = new GenerarPresupuestoPdfCommandHandler(
            _leadRepositoryMock.Object,
            new CalculadoraDeObraService(),
            _pdfGeneratorMock.Object);
    }

    private static GenerarPresupuestoPdfCommand ComandoValido() => new(
        "Ana Restrepo",
        "ana@example.com",
        "3109876543",
        null,
        PropiedadDeInteresId: null,
        DatosCalculoObra: new DatosCalculoObraInput(100, TipoAcabado.Basico, "Gómez Plata", TipoProyecto.Vivienda));

    [Fact]
    public async Task Handle_PersisteUnLeadYaCalificado()
    {
        var comando = ComandoValido();

        await _sut.Handle(comando, CancellationToken.None);

        _leadRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<Lead>(lead =>
                lead.Nombre == "Ana Restrepo" &&
                lead.Estado == EstadoLead.Calificado &&
                lead.ResultadoCalculadora != null),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_LlamaAlGeneradorDePdfConElLeadPersistido()
    {
        var comando = ComandoValido();

        await _sut.Handle(comando, CancellationToken.None);

        _pdfGeneratorMock.Verify(g => g.Generar(It.Is<Lead>(lead => lead.Nombre == "Ana Restrepo")), Times.Once);
    }

    [Fact]
    public async Task Handle_DevuelveLosBytesDelPdfYElNombreDeArchivoConElIdDelLead()
    {
        var comando = ComandoValido();

        var resultado = await _sut.Handle(comando, CancellationToken.None);

        resultado.PdfBytes.Should().Equal(1, 2, 3);
        resultado.Estado.Should().Be(nameof(EstadoLead.Calificado));
        resultado.FileName.Should().Be($"presupuesto-{resultado.LeadId:N}.pdf");
    }
}
