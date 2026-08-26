using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ApplicationValidationException = Plataforma.Application.Common.Exceptions.ValidationException;

namespace Plataforma.WebApi.ErrorHandling;

// Traduce la ValidationException lanzada por ValidationBehaviour (pipeline de
// MediatR, Fase 3) — o por la validación manual en el endpoint de budgets,
// que no pasa por MediatR (Prompt 6, ítem 3) — a un 400 Bad Request con el
// formato estándar ValidationProblemDetails, tal como lo espera el frontend
// (ver frontend/src/types/api.ts).
public sealed class ApplicationExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ApplicationExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ApplicationValidationException validationException)
            return false;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = validationException,
            ProblemDetails = new ValidationProblemDetails(validationException.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Se produjeron uno o más errores de validación.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            },
        });
    }
}
