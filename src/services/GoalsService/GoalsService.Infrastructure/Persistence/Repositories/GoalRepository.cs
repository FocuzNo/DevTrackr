using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Domain.Goals;
using Microsoft.EntityFrameworkCore;

namespace GoalsService.Infrastructure.Persistence.Repositories;

public sealed class GoalRepository(GoalsDbContext dbContext) : IGoalRepository
{
    public Task AddAsync(Goal goal, CancellationToken cancellationToken = default) =>
        dbContext.Goals.AddAsync(goal, cancellationToken).AsTask();

    public Task<Goal?> GetByIdAsync(Guid goalId, Guid userId, CancellationToken cancellationToken = default) =>
        dbContext.Goals.FirstOrDefaultAsync(x => x.Id == goalId && x.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<Goal>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.Goals
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(cancellationToken);
}
