using FluentAssertions;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Propiedades;
using Plataforma.Domain.SharedKernel;
using Plataforma.Domain.ViabilidadAmbiental;
using Plataforma.Domain.ViabilidadAmbiental.Events;
using Plataforma.Domain.ViabilidadAmbiental.Exceptions;
using Plataforma.Domain.ViabilidadAmbiental.ValueObjects;
using Xunit;

namespace Plataforma.Domain.Tests.ViabilidadAmbiental;

public sealed class SolicitudViabilidadAmbientalTests
{
    private static DatosSolicitante CrearSolicitante() => DatosSolicitante.Crear(
        "Ana Restrepo", Email.Crear("ana@example.com"), Telefono.Crear("3109876543"));

    private static UbicacionLote CrearUbicacion() => UbicacionLote.Crear("Antioquia", "Rionegro", "Vereda La Primavera");

    [Fact]
    public void Solicitar_ConUbicacionDeLote_CreaLaSolicitudEnEstadoSolicitadaYDisparaEvento()
    {
        var solicitud = SolicitudViabilidadAmbiental.Solicitar(
            CrearSolicitante(), Dinero.Crear(200_000m), ubicacionLote: CrearUbicacion());

        solicitud.Estado.Should().Be(EstadoSolicitudViabilidad.Solicitada);
        solicitud.PropiedadId.Should().BeNull();
        solicitud.UbicacionLote.Should().NotBeNull();
        solicitud.DomainEvents.Should().ContainSingle(e => e is ViabilidadAmbientalSolicitadaEvent);
    }

    [Fact]
    public void Solicitar_ConPropiedadId_CreaLaSolicitudSinUbicacionLote()
    {
        var propiedadId = PropiedadId.Nueva();

        var solicitud = SolicitudViabilidadAmbiental.Solicitar(
            CrearSolicitante(), Dinero.Crear(200_000m), propiedadId: propiedadId);

        solicitud.PropiedadId.Should().Be(propiedadId);
        solicitud.UbicacionLote.Should().BeNull();
    }

    [Fact]
    public void Solicitar_SinPropiedadIdNiUbicacionLote_LanzaArgumentException()
    {
        var act = () => SolicitudViabilidadAmbiental.Solicitar(CrearSolicitante(), Dinero.Crear(200_000m));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ConfirmarPago_ConSolicitudEnSolicitada_TransicionaAPagadaYDisparaEvento()
    {
        var solicitud = SolicitudViabilidadAmbiental.Solicitar(
            CrearSolicitante(), Dinero.Crear(200_000m), ubicacionLote: CrearUbicacion());
        solicitud.ClearDomainEvents();

        solicitud.ConfirmarPago();

        solicitud.Estado.Should().Be(EstadoSolicitudViabilidad.Pagada);
        solicitud.PagoConfirmadoEn.Should().NotBeNull();
        solicitud.PagoConfirmadoEn.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        solicitud.DomainEvents.Should().ContainSingle(e => e is ViabilidadAmbientalPagoConfirmadoEvent);
    }

    [Fact]
    public void ConfirmarPago_ConSolicitudYaPagada_LanzaEstadoSolicitudViabilidadInvalidoException()
    {
        var solicitud = SolicitudViabilidadAmbiental.Solicitar(
            CrearSolicitante(), Dinero.Crear(200_000m), ubicacionLote: CrearUbicacion());
        solicitud.ConfirmarPago();

        var act = solicitud.ConfirmarPago;

        act.Should().Throw<EstadoSolicitudViabilidadInvalidoException>();
    }

    [Fact]
    public void ConfirmarPago_ConSolicitudRechazada_LanzaEstadoSolicitudViabilidadInvalidoException()
    {
        var solicitud = SolicitudViabilidadAmbiental.Solicitar(
            CrearSolicitante(), Dinero.Crear(200_000m), ubicacionLote: CrearUbicacion());
        solicitud.Rechazar("El monto transferido no coincide.");

        var act = solicitud.ConfirmarPago;

        act.Should().Throw<EstadoSolicitudViabilidadInvalidoException>();
    }

    [Fact]
    public void Rechazar_ConSolicitudEnSolicitada_TransicionaARechazada()
    {
        var solicitud = SolicitudViabilidadAmbiental.Solicitar(
            CrearSolicitante(), Dinero.Crear(200_000m), ubicacionLote: CrearUbicacion());

        solicitud.Rechazar("Nunca llegó la transferencia.");

        solicitud.Estado.Should().Be(EstadoSolicitudViabilidad.Rechazada);
    }

    [Fact]
    public void Rechazar_ConMotivoVacio_LanzaArgumentException()
    {
        var solicitud = SolicitudViabilidadAmbiental.Solicitar(
            CrearSolicitante(), Dinero.Crear(200_000m), ubicacionLote: CrearUbicacion());

        var act = () => solicitud.Rechazar("   ");

        act.Should().Throw<ArgumentException>();
    }
}
