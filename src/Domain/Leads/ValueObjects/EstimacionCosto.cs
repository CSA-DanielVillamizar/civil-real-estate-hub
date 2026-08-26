using Plataforma.Domain.Common;
using Plataforma.Domain.SharedKernel;

namespace Plataforma.Domain.Leads.ValueObjects;

public sealed class DesgloseItem : ValueObject
{
    public string Categoria { get; }
    public Dinero Monto { get; }

    // Reservado para materialización de EF Core (Fase 4).
    private DesgloseItem() { }

    private DesgloseItem(string categoria, Dinero monto)
    {
        Categoria = categoria;
        Monto = monto;
    }

    public static DesgloseItem Crear(string categoria, Dinero monto)
    {
        if (string.IsNullOrWhiteSpace(categoria))
            throw new ArgumentException("La categoría es obligatoria.", nameof(categoria));

        ArgumentNullException.ThrowIfNull(monto);

        return new DesgloseItem(categoria.Trim(), monto);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Categoria;
        yield return Monto;
    }
}

public sealed class EstimacionCosto : ValueObject
{
    // Campo explícito (en vez de un auto-property de solo lectura) para que EF
    // Core pueda hidratar la colección owned "Desglose" por reflexión de campo
    // (ver LeadConfiguration.ConfigurarResultadoCalculadora, Fase 4) — un
    // auto-property get-only no expone un campo con nombre predecible.
    private readonly List<DesgloseItem> _desglose;

    public Dinero MontoMinimo { get; }
    public Dinero MontoMaximo { get; }
    public DatosCalculoObra DatosEntrada { get; }
    public IReadOnlyList<DesgloseItem> Desglose => _desglose.AsReadOnly();

    // Marca cuándo se calculó el snapshot (dato de negocio legítimo para un
    // "hecho histórico", ver Fase 1 §3.1) y, de paso, resuelve un requisito
    // técnico de EF Core: un owned type opcional cuyo contenido son solo
    // sub-objetos anidados no tiene ninguna columna propia con la que
    // distinguir "no hay estimación" de "todas las columnas anidadas son
    // NULL" — EF exige al menos una propiedad escalar propia y no-nula
    // (ver docs/01-domain-model.md, nota de Fase 4).
    public DateTimeOffset CalculadoEn { get; }

    // Reservado para materialización de EF Core (Fase 4).
    private EstimacionCosto() { }

    private EstimacionCosto(
        Dinero montoMinimo,
        Dinero montoMaximo,
        DatosCalculoObra datosEntrada,
        IReadOnlyList<DesgloseItem> desglose,
        DateTimeOffset calculadoEn)
    {
        MontoMinimo = montoMinimo;
        MontoMaximo = montoMaximo;
        DatosEntrada = datosEntrada;
        _desglose = desglose.ToList();
        CalculadoEn = calculadoEn;
    }

    public static EstimacionCosto Crear(Dinero montoMinimo, Dinero montoMaximo, DatosCalculoObra datosEntrada, IReadOnlyList<DesgloseItem> desglose)
    {
        ArgumentNullException.ThrowIfNull(montoMinimo);
        ArgumentNullException.ThrowIfNull(montoMaximo);
        ArgumentNullException.ThrowIfNull(datosEntrada);
        ArgumentNullException.ThrowIfNull(desglose);

        if (montoMaximo.Monto < montoMinimo.Monto)
            throw new ArgumentException("El monto máximo no puede ser menor que el monto mínimo.", nameof(montoMaximo));

        return new EstimacionCosto(montoMinimo, montoMaximo, datosEntrada, desglose, DateTimeOffset.UtcNow);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return MontoMinimo;
        yield return MontoMaximo;
        yield return DatosEntrada;
        foreach (var item in Desglose)
            yield return item;
    }
}
