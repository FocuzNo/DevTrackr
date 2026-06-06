using ActivityService.Application.Sessions.Queries;
using FluentValidation;

namespace ActivityService.Application.Sessions.Validators;

public sealed class GetStudySessionsByDateRangeQueryValidator : AbstractValidator<GetStudySessionsByDateRangeQuery>
{
    public GetStudySessionsByDateRangeQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.To)
            .GreaterThanOrEqualTo(x => x.From)
            .WithMessage("'to' must be greater than or equal to 'from'.");
    }
}
