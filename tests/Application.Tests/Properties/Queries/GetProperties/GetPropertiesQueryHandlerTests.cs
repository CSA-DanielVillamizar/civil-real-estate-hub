using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Properties;
using Plataforma.Application.Properties.Queries.GetProperties;
using Plataforma.Domain.Propiedades;
using Xunit;

namespace Plataforma.Application.Tests.Properties.Queries.GetProperties;

public sealed class GetPropertiesQueryHandlerTests
{
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly GetPropertiesQueryHandler _sut;

    public GetPropertiesQueryHandlerTests()
    {
        _propertyRepositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<PropertyFilter>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Array.Empty<Propiedad>(), 0));

        _sut = new GetPropertiesQueryHandler(_propertyRepositoryMock.Object);
    }

    // Regresión de seguridad: este endpoint es público (sin
    // AdminApiKeyEndpointFilter — ver PropertiesEndpoints). Se detectó en
    // verificación manual que, sin este filtro forzado, un Borrador recién
    // creado quedaba visible en el catálogo público antes de publicarse.
    [Fact]
    public async Task Handle_SiempreFuerzaElFiltroDeEstadoAPublicada_SinImportarOtrosFiltros()
    {
        var query = new GetPropertiesQuery(TipoInmueble.Lote, "Rionegro", null, null, null, null, null, 1, 20);

        await _sut.Handle(query, CancellationToken.None);

        _propertyRepositoryMock.Verify(r => r.SearchAsync(
            It.Is<PropertyFilter>(f => f.Estado == EstadoPropiedad.Publicada),
            1, 20, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
