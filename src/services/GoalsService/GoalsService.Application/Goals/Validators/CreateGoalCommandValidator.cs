using FluentValidation;
using GoalsService.Application.Goals.Commands;

namespace GoalsService.Application.Goals.Validators;

public sealed class CreateGoalCommandValidator : AbstractValidator<CreateGoalCommand>
{
    public CreateGoalCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.TargetMinutes).GreaterThan(0);
        RuleFor(x => x.Deadline)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("Deadline cannot be earlier than start date.");
    }
}
