using GoalsService.Domain.Goals;

namespace GoalsService.Application.Goals.Responses;

public sealed record GoalListItemResponse(
    Guid Id,
    string Title,
    GoalCategory Category,
    int TargetMinutes,
    int CurrentMinutes,
    decimal ProgressPercentage,
    DateOnly StartDate,
    DateOnly Deadline,
    GoalStatus Status,
    DateTime UpdatedAt);
