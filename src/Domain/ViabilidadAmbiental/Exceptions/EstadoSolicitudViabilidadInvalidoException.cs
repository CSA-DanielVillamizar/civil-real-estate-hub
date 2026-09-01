using Plataforma.Domain.Common;

namespace Plataforma.Domain.ViabilidadAmbiental.Exceptions;

public sealed class EstadoSolicitudViabilidadInvalidoException : DomainException
{
    public EstadoSolicitudViabilidadInvalidoException(string message) : base(message) { }
}
