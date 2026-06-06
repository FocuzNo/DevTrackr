using StatisticsService.Domain.Statistics;

namespace StatisticsService.Application.Abstractions.Persistence;

public interface IStatisticsReadRepository
{
    Task<UserStatistics?> GetUserStatisticsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TopicStatistics>> GetTopicStatisticsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DailyStatistics>> GetDailyStatisticsAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);
}
