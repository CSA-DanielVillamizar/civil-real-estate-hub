using Plataforma.Domain.Common;

namespace Plataforma.Domain.Obras.Exceptions;

public sealed class HitoNoEncontradoException : DomainException
{
    public HitoNoEncontradoException(string message) : base(message) { }
}
