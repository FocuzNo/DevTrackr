using GoalsService.Domain.Goals;

namespace GoalsService.Application.Goals.Responses;

public sealed record GoalResponse(
    Guid Id,
    Guid UserId,
    string Title,
    string? Description,
    GoalCategory Category,
    int TargetMinutes,
    int CurrentMinutes,
    decimal ProgressPercentage,
    DateOnly StartDate,
    DateOnly Deadline,
    GoalStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? CompletedAt);
