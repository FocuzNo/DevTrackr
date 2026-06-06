namespace DevTrackr.SharedKernel.Primitives;

public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error Validation(string code, string message) => new(code, message);

    public static Error Failure(string code, string message) => new(code, message);
}
