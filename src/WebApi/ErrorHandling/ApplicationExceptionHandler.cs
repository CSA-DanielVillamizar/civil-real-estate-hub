using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Domain.Common;
using ApplicationValidationException = Plataforma.Application.Common.Exceptions.ValidationException;

namespace Plataforma.WebApi.ErrorHandling;

// Traduce dos familias de excepciones distintas a 400 Bad Request:
//  - ApplicationValidationException: FluentValidation detectó una entrada mal
//    formada (pipeline de MediatR o validación manual en /budgets/calculate,
//    Prompt 6) → ValidationProblemDetails con errores por campo.
//  - DomainException: un invariante de negocio del dominio fue violado (ej.
//    Lead.CalificarPorDescargaDePdf en estado inválido) → ProblemDetails
//    simple con el mensaje de la excepción como Detail.
// Cualquier otra excepción no manejada cae al 500 + ProblemDetails genérico
// que ya provee AddProblemDetails() en Program.cs.
public sealed class ApplicationExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ApplicationExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        return exception switch
        {
            ApplicationValidationException validationException => await HandleValidationExceptionAsync(httpContext, validationException),
            DomainException domainException => await HandleDomainExceptionAsync(httpContext, domainException),
            _ => false,
        };
    }

    private async Task<bool> HandleValidationExceptionAsync(HttpContext httpContext, ApplicationValidationException exception)
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ValidationProblemDetails(exception.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Se produjeron uno o más errores de validación.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            },
        });
    }

    private async Task<bool> HandleDomainExceptionAsync(HttpContext httpContext, DomainException exception)
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Se violó una regla de negocio.",
                Detail = exception.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            },
        });
    }
}
