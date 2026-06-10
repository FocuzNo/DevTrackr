using System.Diagnostics;
using DevTrackr.SharedKernel.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevTrackr.Observability.Http;

public static class ProblemDetailsHttpResultExtensions
{
    public static IResult ToProblemResult(this Error error)
    {
        if (string.Equals(error.Code, "Validation", StringComparison.OrdinalIgnoreCase))
        {
            return CreateValidationProblem(
                title: "Validation Failed",
                errorCode: "validation_error",
                errors: new Dictionary<string, string[]>
                {
                    ["request"] = [error.Message]
                });
        }

        if (error.Code.EndsWith("NotFound", StringComparison.OrdinalIgnoreCase))
        {
            return CreateProblem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource Not Found",
                errorCode: error.Code);
        }

        return CreateProblem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Request Failed",
            errorCode: error.Code,
            detail: error.Message);
    }

    public static IResult CreateProblem(
        int statusCode,
        string title,
        string errorCode,
        string? detail = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = $"https://httpstatuses.com/{statusCode}",
            Detail = detail
        };

        Enrich(problemDetails, errorCode);
        return Results.Problem(problemDetails);
    }

    public static IResult CreateValidationProblem(
        string title,
        string errorCode,
        IDictionary<string, string[]> errors)
    {
        var problemDetails = new HttpValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = title,
            Type = "https://httpstatuses.com/400"
        };

        Enrich(problemDetails, errorCode);
        return Results.ValidationProblem(problemDetails.Errors, title: problemDetails.Title, type: problemDetails.Type, extensions: problemDetails.Extensions);
    }

    private static void Enrich(
        ProblemDetails problemDetails,
        string errorCode)
    {
        problemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString() ?? string.Empty;
        problemDetails.Extensions["errorCode"] = errorCode;
    }
}
