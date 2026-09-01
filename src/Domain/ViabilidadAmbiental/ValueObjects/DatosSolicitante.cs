using Plataforma.Domain.Common;
using Plataforma.Domain.Leads.ValueObjects;

namespace Plataforma.Domain.ViabilidadAmbiental.ValueObjects;

// Reutiliza Email/Telefono de Domain.Leads: son Value Objects de invariante
// puramente formal (regex de formato), sin comportamiento específico del
// contexto Leads — mismo criterio ya usado por Lead al referenciar
// Propiedades.PropiedadId directamente en vez de duplicar el tipo.
public sealed class DatosSolicitante : ValueObject
{
    public string Nombre { get; }
    public Email Email { get; }
    public Telefono Telefono { get; }

    // Reservado para materialización de EF Core.
    private DatosSolicitante() { }

    private DatosSolicitante(string nombre, Email email, Telefono telefono)
    {
        Nombre = nombre;
        Email = email;
        Telefono = telefono;
    }

    public static DatosSolicitante Crear(string nombre, Email email, Telefono telefono)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del solicitante es obligatorio.", nameof(nombre));

        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(telefono);

        return new DatosSolicitante(nombre.Trim(), email, telefono);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Nombre;
        yield return Email;
        yield return Telefono;
    }

    public override string ToString() => $"{Nombre} ({Email})";
}
