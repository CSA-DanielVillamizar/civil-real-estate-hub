namespace Plataforma.Domain.Common;

// Base para excepciones que representan la violación de un invariante de
// negocio (a diferencia de FluentValidation, que valida la FORMA de un
// input antes de llegar al dominio). El manejador global de excepciones
// (WebApi) las traduce a 400 — ver ApplicationExceptionHandler.
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}
