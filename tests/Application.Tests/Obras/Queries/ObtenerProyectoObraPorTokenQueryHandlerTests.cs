using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Obras.Queries.ObtenerProyectoObraPorToken;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Obras;
using Xunit;

namespace Plataforma.Application.Tests.Obras.Queries;

public sealed class ObtenerProyectoObraPorTokenQueryHandlerTests
{
    private readonly Mock<IProyectoObraRepository> _repositoryMock = new();
    private readonly ObtenerProyectoObraPorTokenQueryHandler _sut;

    public ObtenerProyectoObraPorTokenQueryHandlerTests()
    {
        _sut = new ObtenerProyectoObraPorTokenQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConTokenValido_DevuelveElDetalleConSusHitos()
    {
        var proyecto = ProyectoObra.Crear(
            "Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"), "Interventoría casa campestre");
        proyecto.AgregarHito("Cimentación", null, null);
        _repositoryMock.Setup(r => r.GetByTokenAsync(proyecto.TokenAcceso, It.IsAny<CancellationToken>())).ReturnsAsync(proyecto);

        var resultado = await _sut.Handle(new ObtenerProyectoObraPorTokenQuery(proyecto.TokenAcceso), CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado!.NombreProyecto.Should().Be("Interventoría casa campestre");
        resultado.Hitos.Should().ContainSingle(h => h.Nombre == "Cimentación");
    }

    [Fact]
    public async Task Handle_ConTokenInexistente_DevuelveNull()
    {
        _repositoryMock.Setup(r => r.GetByTokenAsync("token-invalido", It.IsAny<CancellationToken>())).ReturnsAsync((ProyectoObra?)null);

        var resultado = await _sut.Handle(new ObtenerProyectoObraPorTokenQuery("token-invalido"), CancellationToken.None);

        resultado.Should().BeNull();
    }
}
