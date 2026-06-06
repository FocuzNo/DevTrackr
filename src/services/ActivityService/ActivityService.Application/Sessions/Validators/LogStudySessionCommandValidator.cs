using ActivityService.Application.Sessions.Commands;
using FluentValidation;

namespace ActivityService.Application.Sessions.Validators;

public sealed class LogStudySessionCommandValidator : AbstractValidator<LogStudySessionCommand>
{
    public LogStudySessionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.GoalId).NotEmpty();
        RuleFor(x => x.Topic).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Note).MaximumLength(1000);
        RuleFor(x => x.DurationMinutes).GreaterThan(0).LessThanOrEqualTo(480);
        RuleFor(x => (int)x.Difficulty).InclusiveBetween(1, 5);
        RuleFor(x => x.SessionDate)
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Session date cannot be in the future.");
    }
}
