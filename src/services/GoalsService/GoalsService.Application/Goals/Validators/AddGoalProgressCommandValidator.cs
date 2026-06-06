using FluentValidation;
using GoalsService.Application.Goals.Commands;

namespace GoalsService.Application.Goals.Validators;

public sealed class AddGoalProgressCommandValidator : AbstractValidator<AddGoalProgressCommand>
{
    public AddGoalProgressCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.GoalId).NotEmpty();
        RuleFor(x => x.MinutesToAdd).GreaterThan(0);
    }
}
