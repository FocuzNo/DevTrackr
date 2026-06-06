using GoalsService.Application.Goals.Responses;

namespace GoalsService.Application.Abstractions.Persistence;

public interface IGoalReadRepository
{
    Task<GoalResponse?> GetByIdAsync(
        Guid goalId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GoalListItemResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
