using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.Properties.Queries.GetPropertiesAdmin;
using Plataforma.Domain.Propiedades;
using Xunit;

namespace Plataforma.Application.Tests.Properties.Queries.GetPropertiesAdmin;

public sealed class GetPropertiesAdminQueryHandlerTests
{
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly GetPropertiesAdminQueryHandler _sut;

    public GetPropertiesAdminQueryHandlerTests()
    {
        _propertyRepositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<PropertyFilter>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Array.Empty<Propiedad>(), 0));

        _sut = new GetPropertiesAdminQueryHandler(_propertyRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_SinEstado_NoFiltraPorEstado_MuestraBorradoresTambien()
    {
        await _sut.Handle(new GetPropertiesAdminQuery(Estado: null), CancellationToken.None);

        _propertyRepositoryMock.Verify(r => r.SearchAsync(
            It.Is<PropertyFilter>(f => f.Estado == null),
            1, 20, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ConEstado_LoPasaTalCualAlRepositorio()
    {
        await _sut.Handle(new GetPropertiesAdminQuery(EstadoPropiedad.Borrador), CancellationToken.None);

        _propertyRepositoryMock.Verify(r => r.SearchAsync(
            It.Is<PropertyFilter>(f => f.Estado == EstadoPropiedad.Borrador),
            1, 20, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
