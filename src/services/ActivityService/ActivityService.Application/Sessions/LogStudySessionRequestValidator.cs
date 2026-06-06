using FluentValidation;

namespace ActivityService.Application.Sessions;

public sealed class LogStudySessionRequestValidator : AbstractValidator<LogStudySessionRequest>
{
    public LogStudySessionRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.GoalId).NotEmpty();
        RuleFor(x => x.Topic).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
    }
}
