using FluentValidation;
using MediatR;
using ApplicationValidationException = Plataforma.Application.Common.Exceptions.ValidationException;

namespace Plataforma.Application.Common.Behaviours;

// Se registra para TODO IRequest con validadores asociados (comandos y queries),
// incluyendo GetPropertiesQuery — así la validación de filtros de un GET queda
// en la capa de Aplicación en vez de depender del model-binding automático de
// ASP.NET Core, que no aplica FluentValidation a parámetros de query por defecto.
public sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count != 0)
            throw new ApplicationValidationException(failures);

        return await next();
    }
}
