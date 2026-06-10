using DevTrackr.Observability.Http;
using DevTrackr.SharedKernel.Primitives;

namespace GoalsService.Api.Extensions;

public static class ResultHttpExtensions
{
    public static IResult ToApiResult<TValue>(this Result<TValue> result, Func<TValue, IResult> onSuccess)
    {
        if (result.IsSuccess && result.Value is not null)
        {
            return onSuccess(result.Value);
        }

        return MapFailure(result.Error);
    }

    public static IResult ToApiResult<TValue>(this Result<TValue> result)
    {
        if (result.IsSuccess && result.Value is not null)
        {
            return Results.Ok(result.Value);
        }

        return MapFailure(result.Error);
    }

    public static IResult ToApiResult(this Result result)
    {
        return result.IsSuccess ? Results.NoContent() : MapFailure(result.Error);
    }

    private static IResult MapFailure(Error error) => error.ToProblemResult();
}
