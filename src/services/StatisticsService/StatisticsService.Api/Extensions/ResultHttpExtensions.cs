using DevTrackr.SharedKernel.Primitives;

namespace StatisticsService.Api.Extensions;

public static class ResultHttpExtensions
{
    public static IResult ToApiResult<TValue>(this Result<TValue> result)
    {
        if (result.IsSuccess && result.Value is not null)
        {
            return Results.Ok(result.Value);
        }

        return MapFailure(result.Error);
    }

    private static IResult MapFailure(Error error)
    {
        if (error.Code == "Validation")
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["request"] = [error.Message]
            });
        }

        return Results.BadRequest(new { error.Code, error.Message });
    }
}
