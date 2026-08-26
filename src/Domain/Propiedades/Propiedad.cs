using Plataforma.Domain.Common;
using Plataforma.Domain.Propiedades.Events;
using Plataforma.Domain.Propiedades.Reglas;
using Plataforma.Domain.Propiedades.ValueObjects;
using Plataforma.Domain.SharedKernel;

namespace Plataforma.Domain.Propiedades;

public sealed class Propiedad : AggregateRoot<PropiedadId>
{
    private readonly List<ArchivoMultimedia> _multimedia = new();
    private readonly List<RetiroAmbiental> _retirosAmbientales = new();

    public string Titulo { get; private set; }
    public string Descripcion { get; private set; }
    public TipoInmueble TipoInmueble { get; private set; }
    public Dinero Precio { get; private set; }
    public EstadoPropiedad Estado { get; private set; }
    public Ubicacion Ubicacion { get; private set; }
    public Area AreaTerreno { get; private set; }
    public Area? AreaConstruida { get; private set; }
    public CaracteristicasTopograficas CaracteristicasTopograficas { get; private set; }

    public IReadOnlyCollection<ArchivoMultimedia> Multimedia => _multimedia.AsReadOnly();
    public IReadOnlyCollection<RetiroAmbiental> RetirosAmbientales => _retirosAmbientales.AsReadOnly();

    // Reservado para materialización de EF Core (Fase 4) — las propiedades se
    // hidratan por reflexión después de construir la instancia.
    private Propiedad() { }

    private Propiedad(
        PropiedadId id,
        string titulo,
        string descripcion,
        TipoInmueble tipoInmueble,
        Dinero precio,
        Ubicacion ubicacion,
        Area areaTerreno,
        Area? areaConstruida,
        CaracteristicasTopograficas caracteristicasTopograficas) : base(id)
    {
        Titulo = titulo;
        Descripcion = descripcion;
        TipoInmueble = tipoInmueble;
        Precio = precio;
        Ubicacion = ubicacion;
        AreaTerreno = areaTerreno;
        AreaConstruida = areaConstruida;
        CaracteristicasTopograficas = caracteristicasTopograficas;
        Estado = EstadoPropiedad.Borrador;
    }

    public static Propiedad Crear(
        string titulo,
        string descripcion,
        TipoInmueble tipoInmueble,
        Dinero precio,
        Ubicacion ubicacion,
        Area areaTerreno,
        CaracteristicasTopograficas caracteristicasTopograficas,
        Area? areaConstruida = null)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título es obligatorio.", nameof(titulo));

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción es obligatoria.", nameof(descripcion));

        ArgumentNullException.ThrowIfNull(precio);
        ArgumentNullException.ThrowIfNull(ubicacion);
        ArgumentNullException.ThrowIfNull(areaTerreno);
        ArgumentNullException.ThrowIfNull(caracteristicasTopograficas);

        return new Propiedad(
            PropiedadId.Nueva(),
            titulo.Trim(),
            descripcion.Trim(),
            tipoInmueble,
            precio,
            ubicacion,
            areaTerreno,
            areaConstruida,
            caracteristicasTopograficas);
    }

    public void ActualizarDatosBasicos(string titulo, string descripcion, Dinero precio)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título es obligatorio.", nameof(titulo));

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción es obligatoria.", nameof(descripcion));

        ArgumentNullException.ThrowIfNull(precio);

        Titulo = titulo.Trim();
        Descripcion = descripcion.Trim();
        Precio = precio;

        AddDomainEvent(new PropiedadActualizadaEvent(Id));
    }

    public void AgregarMultimedia(string url, TipoMultimedia tipo)
    {
        var archivo = ArchivoMultimedia.Crear(url, tipo, _multimedia.Count);
        _multimedia.Add(archivo);
    }

    public void AgregarRetiro(RetiroAmbiental retiro)
    {
        ArgumentNullException.ThrowIfNull(retiro);

        if (_retirosAmbientales.Contains(retiro))
            return;

        _retirosAmbientales.Add(retiro);
    }

    public void RemoverRetiro(RetiroAmbiental retiro) => _retirosAmbientales.Remove(retiro);

    public void Publicar()
    {
        if (Estado is not (EstadoPropiedad.Borrador or EstadoPropiedad.Retirada))
            throw new InvalidOperationException($"No se puede publicar una propiedad en estado {Estado}.");

        if (Precio.Monto <= 0)
            throw new InvalidOperationException("No se puede publicar una propiedad sin precio.");

        if (_multimedia.Count == 0)
            throw new InvalidOperationException("No se puede publicar una propiedad sin al menos un archivo multimedia.");

        Estado = EstadoPropiedad.Publicada;
        AddDomainEvent(new PropiedadPublicadaEvent(Id));
    }

    public void Reservar()
    {
        if (Estado != EstadoPropiedad.Publicada)
            throw new InvalidOperationException($"No se puede reservar una propiedad en estado {Estado}.");

        Estado = EstadoPropiedad.Reservada;
        AddDomainEvent(new PropiedadReservadaEvent(Id));
    }

    public void MarcarVendida()
    {
        if (Estado is not (EstadoPropiedad.Reservada or EstadoPropiedad.Publicada))
            throw new InvalidOperationException($"No se puede marcar vendida una propiedad en estado {Estado}.");

        Estado = EstadoPropiedad.Vendida;
        AddDomainEvent(new PropiedadVendidaEvent(Id, Ubicacion.Municipio));
    }

    public void MarcarArrendada()
    {
        if (Estado is not (EstadoPropiedad.Reservada or EstadoPropiedad.Publicada))
            throw new InvalidOperationException($"No se puede marcar arrendada una propiedad en estado {Estado}.");

        Estado = EstadoPropiedad.Arrendada;
        AddDomainEvent(new PropiedadActualizadaEvent(Id));
    }

    public void Retirar()
    {
        if (Estado is EstadoPropiedad.Vendida)
            throw new InvalidOperationException("No se puede retirar una propiedad ya vendida.");

        Estado = EstadoPropiedad.Retirada;
        AddDomainEvent(new PropiedadRetiradaEvent(Id));
    }

    // Cálculo puro, sin efectos secundarios — seguro de invocar en rutas de
    // solo lectura (ej. listar propiedades filtrando por viabilidad).
    public ResultadoViabilidad CalcularViabilidadConstructiva()
    {
        var restricciones = new List<string>();

        if (CaracteristicasTopograficas.PendientePorcentaje > ViabilidadConstructivaReglas.PendienteMaximaPermitidaPorcentaje)
        {
            restricciones.Add(
                $"Pendiente del terreno ({CaracteristicasTopograficas.PendientePorcentaje}%) supera el máximo de referencia " +
                $"({ViabilidadConstructivaReglas.PendienteMaximaPermitidaPorcentaje}%).");
        }

        foreach (var retiro in _retirosAmbientales)
        {
            restricciones.Add(
                $"Aplica retiro ambiental de {retiro.TipoFuente} (mínimo {retiro.DistanciaMinimaMetros}m, " +
                $"normativa: {retiro.NormativaAplicable}) — verificar cumplimiento en el diseño constructivo.");
        }

        var esViable = CaracteristicasTopograficas.PendientePorcentaje <= ViabilidadConstructivaReglas.PendienteMaximaPermitidaPorcentaje;

        return ResultadoViabilidad.Crear(esViable, restricciones);
    }

    // Evalúa la viabilidad Y deja constancia del hecho como evento de dominio
    // (ej. para auditoría o notificación). Usar solo en flujos de escritura
    // explícitos — no en listados/consultas (ver CalcularViabilidadConstructiva).
    public ResultadoViabilidad EvaluarViabilidadConstructiva()
    {
        var resultado = CalcularViabilidadConstructiva();
        AddDomainEvent(new ViabilidadConstructivaEvaluadaEvent(Id, resultado.EsViable, resultado.Restricciones));
        return resultado;
    }
}
