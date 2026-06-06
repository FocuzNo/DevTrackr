using GoalsService.Domain.Goals;

namespace GoalsService.Application.Goals.Commands;

public sealed record UpdateGoalCommand(
    Guid UserId,
    Guid GoalId,
    string Title,
    string? Description,
    GoalCategory Category,
    int TargetMinutes,
    DateOnly StartDate,
    DateOnly Deadline);
