namespace DevTrackr.SharedKernel.Primitives;

public sealed class Result<TValue> : Result
{
    private Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public TValue? Value { get; }

    public static Result<TValue> Success(TValue value) => new(value, true, Error.None);

    public static new Result<TValue> Failure(Error error) => new(default, false, error);
}
