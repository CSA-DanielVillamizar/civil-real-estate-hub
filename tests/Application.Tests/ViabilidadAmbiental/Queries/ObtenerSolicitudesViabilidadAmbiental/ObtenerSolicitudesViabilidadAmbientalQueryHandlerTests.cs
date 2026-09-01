using FluentAssertions;
using Moq;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Application.ViabilidadAmbiental.Queries.ObtenerSolicitudesViabilidadAmbiental;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.SharedKernel;
using Plataforma.Domain.ViabilidadAmbiental;
using Plataforma.Domain.ViabilidadAmbiental.ValueObjects;
using Xunit;

namespace Plataforma.Application.Tests.ViabilidadAmbiental.Queries.ObtenerSolicitudesViabilidadAmbiental;

public sealed class ObtenerSolicitudesViabilidadAmbientalQueryHandlerTests
{
    private readonly Mock<ISolicitudViabilidadAmbientalRepository> _repositoryMock = new();
    private readonly ObtenerSolicitudesViabilidadAmbientalQueryHandler _sut;

    public ObtenerSolicitudesViabilidadAmbientalQueryHandlerTests()
    {
        _sut = new ObtenerSolicitudesViabilidadAmbientalQueryHandler(_repositoryMock.Object);
    }

    private static SolicitudViabilidadAmbiental CrearSolicitud() => SolicitudViabilidadAmbiental.Solicitar(
        DatosSolicitante.Crear("Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543")),
        Dinero.Crear(200_000m),
        ubicacionLote: UbicacionLote.Crear("Antioquia", "Rionegro"));

    [Fact]
    public async Task Handle_MapeaCadaSolicitudAUnListItemConSusDatos()
    {
        var solicitud = CrearSolicitud();
        _repositoryMock
            .Setup(r => r.ListAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync([solicitud]);

        var resultado = await _sut.Handle(new ObtenerSolicitudesViabilidadAmbientalQuery(null), CancellationToken.None);

        resultado.Should().ContainSingle();
        var item = resultado[0];
        item.Id.Should().Be(solicitud.Id.Value);
        item.Nombre.Should().Be("Ana Restrepo");
        item.Email.Should().Be("ana@example.com");
        item.Municipio.Should().Be("Rionegro");
        item.PropiedadId.Should().BeNull();
        item.Monto.Should().Be(200_000m);
        item.Estado.Should().Be(nameof(EstadoSolicitudViabilidad.Solicitada));
    }

    [Fact]
    public async Task Handle_PasaElFiltroDeEstadoAlRepositorio()
    {
        _repositoryMock
            .Setup(r => r.ListAsync(EstadoSolicitudViabilidad.Pagada, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        await _sut.Handle(new ObtenerSolicitudesViabilidadAmbientalQuery(EstadoSolicitudViabilidad.Pagada), CancellationToken.None);

        _repositoryMock.Verify(r => r.ListAsync(EstadoSolicitudViabilidad.Pagada, It.IsAny<CancellationToken>()), Times.Once);
    }
}
