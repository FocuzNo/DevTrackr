using Microsoft.EntityFrameworkCore;
using StatisticsService.Domain.Statistics;

namespace StatisticsService.Infrastructure.Persistence.Repositories;

public sealed class StatisticsProjectionRepository(StatisticsDbContext dbContext) : IStatisticsProjectionRepository
{
    public Task<UserStatistics?> GetUserStatisticsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        dbContext.UserStatistics.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public Task<TopicStatistics?> GetTopicStatisticsAsync(Guid userId, string topic, CancellationToken cancellationToken = default) =>
        dbContext.TopicStatistics.FirstOrDefaultAsync(x => x.UserId == userId && x.Topic == topic, cancellationToken);

    public Task<DailyStatistics?> GetDailyStatisticsAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default) =>
        dbContext.DailyStatistics.FirstOrDefaultAsync(x => x.UserId == userId && x.Date == date, cancellationToken);

    public async Task<IReadOnlyList<DateOnly>> GetStudyDatesAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.DailyStatistics
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Date)
            .Select(x => x.Date)
            .ToListAsync(cancellationToken);

    public Task AddAsync(UserStatistics userStatistics, CancellationToken cancellationToken = default) =>
        dbContext.UserStatistics.AddAsync(userStatistics, cancellationToken).AsTask();

    public Task AddAsync(TopicStatistics topicStatistics, CancellationToken cancellationToken = default) =>
        dbContext.TopicStatistics.AddAsync(topicStatistics, cancellationToken).AsTask();

    public Task AddAsync(DailyStatistics dailyStatistics, CancellationToken cancellationToken = default) =>
        dbContext.DailyStatistics.AddAsync(dailyStatistics, cancellationToken).AsTask();
}
