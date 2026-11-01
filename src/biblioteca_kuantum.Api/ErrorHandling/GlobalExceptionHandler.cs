using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace biblioteca_kuantum.Api.ErrorHandling;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ValidationException validation => CreateValidationProblem(validation),

            _ => Create(StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                "Ocurrió un error inesperado. Intente nuevamente más tarde.")
        };

        var status = problemDetails.Status!.Value;

        if (status >= 500)
            logger.LogError(exception, "Excepción no controlada: {Message}", exception.Message);
        else
            logger.LogWarning("Error de aplicación ({Status}): {Message}", status, exception.Message);

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }

    private static ProblemDetails Create(int status, string title, string detail) => new()
    {
        Status = status,
        Title = title,
        Detail = detail
    };

    private static HttpValidationProblemDetails CreateValidationProblem(ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).Distinct().ToArray());

        return new HttpValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Error de validación",
            Detail = "Se produjeron uno o más errores de validación."
        };
    }
}