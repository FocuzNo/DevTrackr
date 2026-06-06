using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using FluentValidation;
using System.Reflection;

namespace DevTrackr.Cqrs.Validation;

public sealed class ValidationBehavior<TRequest, TResult>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResult>
{
    public async Task<TResult> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResult> next)
    {
        var validatorArray = validators as IValidator<TRequest>[] ?? validators.ToArray();
        if (validatorArray.Length == 0)
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validatorArray.Select(x => x.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(x => x.Errors)
            .Where(x => x is not null)
            .ToArray();

        if (failures.Length == 0)
        {
            return await next();
        }

        if (ResultFailureFactory.TryCreateFailure<TResult>(failures, out var result))
        {
            return result;
        }

        throw new ValidationException(failures);
    }
}

internal static class ResultFailureFactory
{
    public static bool TryCreateFailure<TResult>(IEnumerable<FluentValidation.Results.ValidationFailure> failures, out TResult result)
    {
        var error = CreateError(failures);
        var resultType = typeof(TResult);

        if (resultType == typeof(Result))
        {
            result = (TResult)(object)Result.Failure(error);
            return true;
        }

        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = resultType.GetMethod(
                nameof(Result<object>.Failure),
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                [typeof(Error)],
                modifiers: null);

            if (failureMethod is not null)
            {
                result = (TResult)(failureMethod.Invoke(null, [error])
                    ?? throw new InvalidOperationException($"Failed to create validation failure result for '{resultType.FullName}'."));
                return true;
            }
        }

        result = default!;
        return false;
    }

    private static Error CreateError(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
    {
        var message = string.Join(
            "; ",
            failures.Select(x => $"{x.PropertyName}: {x.ErrorMessage}"));

        return Error.Validation("Validation", message);
    }
}
