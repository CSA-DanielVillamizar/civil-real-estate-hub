using Plataforma.Domain.Common;
using Plataforma.Domain.Leads;

namespace Plataforma.Domain.Tarifas;

// Transparencia de precios (gap #5) para consultoría estructural e
// interventoría — las 2 líneas de negocio sin ningún indicio de costo hoy.
// El modelo de cobro varía por línea (por m², % del valor de obra, tarifa
// plana), así que el rango es opcional y la unidad es texto libre en vez de
// un enum cerrado — no hay todavía una convención de negocio que forzar.
public sealed class PaqueteTarifa : AggregateRoot<PaqueteTarifaId>
{
    public ServicioDeInteres ServicioRelacionado { get; private set; }

    public string Titulo { get; private set; }

    public string Descripcion { get; private set; }

    public decimal? PrecioDesde { get; private set; }

    public decimal? PrecioHasta { get; private set; }

    // Ej. "por m²", "% del valor de la obra", "tarifa plana por proyecto".
    public string UnidadPrecio { get; private set; }

    public string Moneda { get; private set; }

    // Arranca en false — un admin revisa el paquete antes de que sea
    // visible en el sitio público (mismo patrón que ContenidoConfianza).
    public bool Publicado { get; private set; }

    public DateTimeOffset CreadoEn { get; private set; }

    // Reservado para materialización de EF Core.
    private PaqueteTarifa() { }

    private PaqueteTarifa(
        PaqueteTarifaId id,
        ServicioDeInteres servicioRelacionado,
        string titulo,
        string descripcion,
        decimal? precioDesde,
        decimal? precioHasta,
        string unidadPrecio,
        string moneda) : base(id)
    {
        ServicioRelacionado = servicioRelacionado;
        Titulo = titulo;
        Descripcion = descripcion;
        PrecioDesde = precioDesde;
        PrecioHasta = precioHasta;
        UnidadPrecio = unidadPrecio;
        Moneda = moneda;
        Publicado = false;
        CreadoEn = DateTimeOffset.UtcNow;
    }

    public static PaqueteTarifa Crear(
        ServicioDeInteres servicioRelacionado,
        string titulo,
        string descripcion,
        decimal? precioDesde,
        decimal? precioHasta,
        string unidadPrecio,
        string moneda)
    {
        ValidarTitulo(titulo);
        ValidarDescripcion(descripcion);
        ValidarUnidadPrecio(unidadPrecio);
        ValidarRango(precioDesde, precioHasta);

        if (string.IsNullOrWhiteSpace(moneda))
            throw new ArgumentException("La moneda es obligatoria.", nameof(moneda));

        return new PaqueteTarifa(
            PaqueteTarifaId.Nueva(), servicioRelacionado, titulo.Trim(), descripcion.Trim(),
            precioDesde, precioHasta, unidadPrecio.Trim(), moneda.Trim().ToUpperInvariant());
    }

    public void Actualizar(
        string titulo, string descripcion, decimal? precioDesde, decimal? precioHasta, string unidadPrecio, ServicioDeInteres servicioRelacionado)
    {
        ValidarTitulo(titulo);
        ValidarDescripcion(descripcion);
        ValidarUnidadPrecio(unidadPrecio);
        ValidarRango(precioDesde, precioHasta);

        Titulo = titulo.Trim();
        Descripcion = descripcion.Trim();
        PrecioDesde = precioDesde;
        PrecioHasta = precioHasta;
        UnidadPrecio = unidadPrecio.Trim();
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

    private static void ValidarUnidadPrecio(string unidadPrecio)
    {
        if (string.IsNullOrWhiteSpace(unidadPrecio))
            throw new ArgumentException("La unidad de precio es obligatoria.", nameof(unidadPrecio));

        if (unidadPrecio.Trim().Length > 100)
            throw new ArgumentException("La unidad de precio no puede superar 100 caracteres.", nameof(unidadPrecio));
    }

    private static void ValidarRango(decimal? precioDesde, decimal? precioHasta)
    {
        if (precioDesde is < 0)
            throw new ArgumentException("El precio desde no puede ser negativo.", nameof(precioDesde));

        if (precioHasta is < 0)
            throw new ArgumentException("El precio hasta no puede ser negativo.", nameof(precioHasta));

        if (precioDesde is not null && precioHasta is not null && precioDesde > precioHasta)
            throw new ArgumentException("El precio desde no puede ser mayor que el precio hasta.", nameof(precioDesde));
    }
}
