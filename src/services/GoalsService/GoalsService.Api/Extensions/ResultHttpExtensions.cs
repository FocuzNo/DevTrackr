using DevTrackr.SharedKernel.Primitives;
using GoalsService.Application.Goals.Responses;

namespace GoalsService.Api.Extensions;

public static class ResultHttpExtensions
{
    public static IResult ToApiResult(this Result<GoalResponse> result, Func<GoalResponse, IResult> onSuccess)
    {
        if (result.IsSuccess && result.Value is not null)
        {
            return onSuccess(result.Value);
        }

        return MapFailure(result.Error);
    }

    public static IResult ToApiResult(this Result result)
    {
        return result.IsSuccess ? Results.NoContent() : MapFailure(result.Error);
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

        if (error.Code == "Goals.GoalNotFound")
        {
            return Results.NotFound(new { error.Code, error.Message });
        }

        return Results.BadRequest(new { error.Code, error.Message });
    }
}
