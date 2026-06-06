using DevTrackr.SharedKernel.Persistence;
using GoalsService.Domain.Goals;

namespace GoalsService.Application.Abstractions.Persistence;

public interface IGoalRepository : IRepository<Goal>
{
    Task<Goal?> GetByIdAsync(
        Guid goalId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
