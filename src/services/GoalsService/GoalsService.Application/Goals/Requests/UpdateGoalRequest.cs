using GoalsService.Domain.Goals;

namespace GoalsService.Application.Goals.Requests;

public sealed record UpdateGoalRequest(
    string Title,
    string? Description,
    GoalCategory Category,
    int TargetMinutes,
    DateOnly StartDate,
    DateOnly Deadline);
