using System.Security.Cryptography;
using Plataforma.Domain.Common;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Obras.Exceptions;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Domain.Obras;

// Portal de avance de obra (P3): el cliente entra con un link único
// (TokenAcceso), sin usuario ni contraseña — mismo espíritu que el resto del
// proyecto (transferencia bancaria en vez de pasarela de pago): la solución
// más simple que resuelve el problema real, no la más "formal". El token es
// la única credencial, así que debe ser criptográficamente aleatorio y largo
// (256 bits) — adivinarlo por fuerza bruta debe ser inviable.
public sealed class ProyectoObra : AggregateRoot<ProyectoObraId>
{
    private readonly List<HitoObra> _hitos = new();

    public string NombreCliente { get; private set; }
    public Email EmailCliente { get; private set; }
    public Telefono TelefonoCliente { get; private set; }
    public string NombreProyecto { get; private set; }
    public string? Descripcion { get; private set; }

    // Referencia débil entre bounded contexts (igual que
    // Lead.PropiedadDeInteresId) — opcional: un proyecto de Consultoría o
    // Interventoría puede no corresponder a ninguna propiedad del catálogo.
    public PropiedadId? PropiedadId { get; private set; }

    public string TokenAcceso { get; private set; }
    public EstadoProyecto Estado { get; private set; }
    public DateTimeOffset CreadoEn { get; private set; }
    public IReadOnlyList<HitoObra> Hitos => _hitos.AsReadOnly();

    // Reservado para materialización de EF Core.
    private ProyectoObra() { }

    private ProyectoObra(
        ProyectoObraId id,
        string nombreCliente,
        Email emailCliente,
        Telefono telefonoCliente,
        string nombreProyecto,
        string? descripcion,
        PropiedadId? propiedadId) : base(id)
    {
        NombreCliente = nombreCliente;
        EmailCliente = emailCliente;
        TelefonoCliente = telefonoCliente;
        NombreProyecto = nombreProyecto;
        Descripcion = descripcion;
        PropiedadId = propiedadId;
        TokenAcceso = GenerarToken();
        Estado = EstadoProyecto.Planificacion;
        CreadoEn = DateTimeOffset.UtcNow;
    }

    public static ProyectoObra Crear(
        string nombreCliente,
        Email emailCliente,
        Telefono telefonoCliente,
        string nombreProyecto,
        string? descripcion = null,
        PropiedadId? propiedadId = null)
    {
        if (string.IsNullOrWhiteSpace(nombreCliente))
            throw new ArgumentException("El nombre del cliente es obligatorio.", nameof(nombreCliente));

        if (string.IsNullOrWhiteSpace(nombreProyecto))
            throw new ArgumentException("El nombre del proyecto es obligatorio.", nameof(nombreProyecto));

        ArgumentNullException.ThrowIfNull(emailCliente);
        ArgumentNullException.ThrowIfNull(telefonoCliente);

        return new ProyectoObra(
            ProyectoObraId.Nueva(), nombreCliente.Trim(), emailCliente, telefonoCliente,
            nombreProyecto.Trim(), descripcion?.Trim(), propiedadId);
    }

    public HitoObra AgregarHito(string nombre, string? descripcion, DateOnly? fechaEstimada)
    {
        var hito = HitoObra.Crear(nombre, descripcion, _hitos.Count, fechaEstimada);
        _hitos.Add(hito);
        return hito;
    }

    public void CambiarEstadoHito(Guid hitoId, EstadoHito nuevoEstado)
    {
        var hito = BuscarHito(hitoId);
        hito.CambiarEstado(nuevoEstado);
    }

    public void AgregarEvidenciaAHito(Guid hitoId, string url)
    {
        var hito = BuscarHito(hitoId);
        hito.AgregarEvidencia(url);
    }

    public void CambiarEstado(EstadoProyecto nuevoEstado)
    {
        Estado = nuevoEstado;
    }

    private HitoObra BuscarHito(Guid hitoId)
    {
        var hito = _hitos.FirstOrDefault(h => h.Id == hitoId);
        if (hito is null)
            throw new HitoNoEncontradoException($"No existe un hito con id {hitoId} en este proyecto.");

        return hito;
    }

    private static string GenerarToken()
    {
        // 32 bytes (256 bits) en Base64Url — nada de caracteres que requieran
        // escape en una URL, y suficiente entropía para que adivinarlo por
        // fuerza bruta sea inviable (es la única credencial de acceso).
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
}
