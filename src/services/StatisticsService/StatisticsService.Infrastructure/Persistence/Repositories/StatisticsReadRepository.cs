using Microsoft.EntityFrameworkCore;
using StatisticsService.Application.Abstractions.Persistence;
using StatisticsService.Domain.Statistics;

namespace StatisticsService.Infrastructure.Persistence.Repositories;

public sealed class StatisticsReadRepository(StatisticsDbContext dbContext) : IStatisticsReadRepository
{
    public Task<UserStatistics?> GetUserStatisticsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        dbContext.UserStatistics.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<TopicStatistics>> GetTopicStatisticsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.TopicStatistics
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.TotalMinutes)
            .ThenBy(x => x.Topic)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<DailyStatistics>> GetDailyStatisticsAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default) =>
        await dbContext.DailyStatistics
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Date >= from && x.Date <= to)
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);
}
