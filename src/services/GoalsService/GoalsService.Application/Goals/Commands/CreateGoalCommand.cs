using GoalsService.Domain.Goals;

namespace GoalsService.Application.Goals.Commands;

public sealed record CreateGoalCommand(
    Guid UserId,
    string Title,
    string? Description,
    GoalCategory Category,
    int TargetMinutes,
    DateOnly StartDate,
    DateOnly Deadline);
