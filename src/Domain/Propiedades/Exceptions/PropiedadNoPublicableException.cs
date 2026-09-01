using Plataforma.Domain.Common;

namespace Plataforma.Domain.Propiedades.Exceptions;

public sealed class PropiedadNoPublicableException : DomainException
{
    public PropiedadNoPublicableException(string message) : base(message) { }
}
