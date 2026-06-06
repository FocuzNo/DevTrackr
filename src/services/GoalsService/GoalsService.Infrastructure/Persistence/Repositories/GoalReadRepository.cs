using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Application.Goals.Responses;
using Microsoft.EntityFrameworkCore;

namespace GoalsService.Infrastructure.Persistence.Repositories;

public sealed class GoalReadRepository(GoalsDbContext dbContext) : IGoalReadRepository
{
    public Task<GoalResponse?> GetByIdAsync(
        Guid goalId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        dbContext.Goals
            .AsNoTracking()
            .Where(x => x.Id == goalId && x.UserId == userId)
            .Select(x => new GoalResponse(
                x.Id,
                x.UserId,
                x.Title,
                x.Description,
                x.Category,
                x.TargetMinutes,
                x.CurrentMinutes,
                x.TargetMinutes <= 0
                    ? 0
                    : decimal.Round((decimal)x.CurrentMinutes / x.TargetMinutes * 100, 2),
                x.StartDate,
                x.Deadline,
                x.Status,
                x.CreatedAt,
                x.UpdatedAt,
                x.CompletedAt))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<GoalListItemResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Goals
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.UpdatedAt)
            .Select(x => new GoalListItemResponse(
                x.Id,
                x.Title,
                x.Category,
                x.TargetMinutes,
                x.CurrentMinutes,
                x.TargetMinutes <= 0
                    ? 0
                    : decimal.Round((decimal)x.CurrentMinutes / x.TargetMinutes * 100, 2),
                x.StartDate,
                x.Deadline,
                x.Status,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);
}
