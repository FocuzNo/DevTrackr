using System.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevTrackr.Observability.ExceptionHandling;

public sealed class GlobalExceptionHandler(
    IHostEnvironment environment,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(x => string.IsNullOrWhiteSpace(x.PropertyName) ? "request" : x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y.ErrorMessage).Distinct().ToArray());

            var validationProblem = new HttpValidationProblemDetails(errors)
            {
                Title = "Validation Failed",
                Status = StatusCodes.Status400BadRequest,
                Type = "https://httpstatuses.com/400"
            };

            Enrich(validationProblem, httpContext, "validation_error");
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            await httpContext.Response.WriteAsJsonAsync(validationProblem, cancellationToken);
            return true;
        }

        logger.LogError(
            exception,
            "Unhandled exception for {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        var problemDetails = new ProblemDetails
        {
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatuses.com/500"
        };

        if (environment.IsDevelopment())
        {
            problemDetails.Detail = exception.Message;
        }

        Enrich(problemDetails, httpContext, "internal_error");
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private static void Enrich(
        ProblemDetails problemDetails,
        HttpContext httpContext,
        string errorCode)
    {
        problemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;
        problemDetails.Extensions["errorCode"] = errorCode;
    }
}
