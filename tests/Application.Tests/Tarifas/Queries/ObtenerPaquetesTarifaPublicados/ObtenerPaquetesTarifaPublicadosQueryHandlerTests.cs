using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Tarifas.Queries.ObtenerPaquetesTarifaPublicados;
using Plataforma.Domain.Leads;
using Plataforma.Domain.Tarifas;
using Xunit;

namespace Plataforma.Application.Tests.Tarifas.Queries.ObtenerPaquetesTarifaPublicados;

public sealed class ObtenerPaquetesTarifaPublicadosQueryHandlerTests
{
    private readonly Mock<IPaqueteTarifaRepository> _repositoryMock = new();
    private readonly ObtenerPaquetesTarifaPublicadosQueryHandler _sut;

    public ObtenerPaquetesTarifaPublicadosQueryHandlerTests()
    {
        _sut = new ObtenerPaquetesTarifaPublicadosQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DevuelveLosItemsQueEntregaElRepositorioDePublicados()
    {
        var paquete = PaqueteTarifa.Crear(ServicioDeInteres.Inmobiliaria, "Título", "Descripción.", null, null, "tarifa plana", "COP");
        paquete.Publicar();
        _repositoryMock.Setup(r => r.ListPublicadosAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { paquete });

        var resultado = await _sut.Handle(new ObtenerPaquetesTarifaPublicadosQuery(), CancellationToken.None);

        resultado.Should().ContainSingle(r => r.Id == paquete.Id.Value && r.Publicado);
    }
}
