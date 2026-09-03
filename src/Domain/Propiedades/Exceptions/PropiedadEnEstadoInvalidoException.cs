using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.Exceptions;

// Reservar/MarcarVendida/MarcarArrendada/Retirar lanzan InvalidOperationException
// (código preexistente, anterior a la convención de DomainException — mismo
// caso que PropiedadNoPublicableException) — se traduce a esta en el borde de
// Application para que ApplicationExceptionHandler la mapee a 400.
public sealed class PropiedadEnEstadoInvalidoException : DomainException
{
    public PropiedadEnEstadoInvalidoException(string message) : base(message) { }
}
