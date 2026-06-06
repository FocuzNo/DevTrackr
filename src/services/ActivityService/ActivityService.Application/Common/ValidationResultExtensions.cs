using DevTrackr.SharedKernel.Primitives;
using FluentValidation.Results;

namespace ActivityService.Application.Common;

internal static class ValidationResultExtensions
{
    public static Error ToError(this ValidationResult validationResult)
    {
        var message = string.Join("; ", validationResult.Errors.Select(x => x.ErrorMessage));
        return Error.Validation("Validation", message);
    }
}
