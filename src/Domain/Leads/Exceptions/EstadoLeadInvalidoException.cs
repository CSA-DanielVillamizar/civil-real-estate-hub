using Plataforma.Domain.Common;

namespace Plataforma.Domain.Leads.Exceptions;

public sealed class EstadoLeadInvalidoException : DomainException
{
    public EstadoLeadInvalidoException(string message) : base(message) { }
}
