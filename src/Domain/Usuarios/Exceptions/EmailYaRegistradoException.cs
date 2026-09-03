using Plataforma.Domain.Common;

namespace Plataforma.Domain.Usuarios.Exceptions;

public sealed class EmailYaRegistradoException : DomainException
{
    public EmailYaRegistradoException(string message) : base(message) { }
}
