using GoalsService.Application.Goals.Responses;
using GoalsService.Domain.Goals;

namespace GoalsService.Application.Goals;

internal static class GoalMappings
{
    public static GoalResponse ToResponse(this Goal goal) =>
        new(
            goal.Id,
            goal.UserId,
            goal.Title,
            goal.Description,
            goal.Category,
            goal.TargetMinutes,
            goal.CurrentMinutes,
            goal.ProgressPercentage,
            goal.StartDate,
            goal.Deadline,
            goal.Status,
            goal.CreatedAt,
            goal.UpdatedAt,
            goal.CompletedAt);

    public static GoalListItemResponse ToListItem(this Goal goal) =>
        new(
            goal.Id,
            goal.Title,
            goal.Category,
            goal.TargetMinutes,
            goal.CurrentMinutes,
            goal.ProgressPercentage,
            goal.StartDate,
            goal.Deadline,
            goal.Status,
            goal.UpdatedAt);
}
