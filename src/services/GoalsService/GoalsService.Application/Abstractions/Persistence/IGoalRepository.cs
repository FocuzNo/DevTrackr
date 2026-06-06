using GoalsService.Domain.Goals;

namespace GoalsService.Application.Abstractions.Persistence;

public interface IGoalRepository
{
    Task AddAsync(Goal goal, CancellationToken cancellationToken = default);

    Task<Goal?> GetByIdAsync(Guid goalId, Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Goal>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
