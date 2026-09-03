using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Confianza.Queries.ObtenerContenidoConfianzaPublicado;
using Plataforma.Domain.Confianza;
using Plataforma.Domain.Leads;
using Xunit;

namespace Plataforma.Application.Tests.Confianza.Queries.ObtenerContenidoConfianzaPublicado;

public sealed class ObtenerContenidoConfianzaPublicadoQueryHandlerTests
{
    private readonly Mock<IContenidoConfianzaRepository> _repositoryMock = new();
    private readonly ObtenerContenidoConfianzaPublicadoQueryHandler _sut;

    public ObtenerContenidoConfianzaPublicadoQueryHandlerTests()
    {
        _sut = new ObtenerContenidoConfianzaPublicadoQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DevuelveLosItemsQueEntregaElRepositorioDePublicados()
    {
        var contenido = ContenidoConfianza.Crear(
            TipoContenidoConfianza.Testimonio, "Ana Restrepo", "Excelente.", null, ServicioDeInteres.Inmobiliaria);
        contenido.Publicar();
        _repositoryMock.Setup(r => r.ListPublicadosAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { contenido });

        var resultado = await _sut.Handle(new ObtenerContenidoConfianzaPublicadoQuery(), CancellationToken.None);

        resultado.Should().ContainSingle(r => r.Id == contenido.Id.Value && r.Publicado);
    }
}
