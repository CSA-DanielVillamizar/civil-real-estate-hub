using Plataforma.Domain.Common;
using Plataforma.Domain.Leads;

namespace Plataforma.Domain.Confianza;

public sealed class ContenidoConfianza : AggregateRoot<ContenidoConfianzaId>
{
    public TipoContenidoConfianza Tipo { get; private set; }

    // Testimonio → nombre del cliente. Portafolio → nombre/título del proyecto.
    public string Titulo { get; private set; }

    // Testimonio → la cita textual. Portafolio → el resumen del caso.
    public string Descripcion { get; private set; }

    // Solo aplica a Portafolio en la práctica (dónde quedó el proyecto);
    // se deja disponible también para Testimonio por si el cliente quiere
    // mencionar la ubicación de su predio — nunca obligatorio.
    public string? Municipio { get; private set; }

    public ServicioDeInteres ServicioRelacionado { get; private set; }

    // Arranca en false igual que Propiedad.Crear/Publicar — un admin revisa
    // el contenido antes de que sea visible en el sitio público.
    public bool Publicado { get; private set; }

    public DateTimeOffset CreadoEn { get; private set; }

    // Reservado para materialización de EF Core.
    private ContenidoConfianza() { }

    private ContenidoConfianza(
        ContenidoConfianzaId id,
        TipoContenidoConfianza tipo,
        string titulo,
        string descripcion,
        string? municipio,
        ServicioDeInteres servicioRelacionado) : base(id)
    {
        Tipo = tipo;
        Titulo = titulo;
        Descripcion = descripcion;
        Municipio = municipio;
        ServicioRelacionado = servicioRelacionado;
        Publicado = false;
        CreadoEn = DateTimeOffset.UtcNow;
    }

    public static ContenidoConfianza Crear(
        TipoContenidoConfianza tipo, string titulo, string descripcion, string? municipio, ServicioDeInteres servicioRelacionado)
    {
        ValidarTitulo(titulo);
        ValidarDescripcion(descripcion);

        return new ContenidoConfianza(
            ContenidoConfianzaId.Nueva(), tipo, titulo.Trim(), descripcion.Trim(), municipio?.Trim(), servicioRelacionado);
    }

    public void Actualizar(string titulo, string descripcion, string? municipio, ServicioDeInteres servicioRelacionado)
    {
        ValidarTitulo(titulo);
        ValidarDescripcion(descripcion);

        Titulo = titulo.Trim();
        Descripcion = descripcion.Trim();
        Municipio = municipio?.Trim();
        ServicioRelacionado = servicioRelacionado;
    }

    public void Publicar() => Publicado = true;

    public void Despublicar() => Publicado = false;

    private static void ValidarTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título es obligatorio.", nameof(titulo));

        if (titulo.Trim().Length > 150)
            throw new ArgumentException("El título no puede superar 150 caracteres.", nameof(titulo));
    }

    private static void ValidarDescripcion(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción es obligatoria.", nameof(descripcion));

        if (descripcion.Trim().Length > 1000)
            throw new ArgumentException("La descripción no puede superar 1000 caracteres.", nameof(descripcion));
    }
}
