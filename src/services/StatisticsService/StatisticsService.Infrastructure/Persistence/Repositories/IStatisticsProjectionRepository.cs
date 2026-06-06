using StatisticsService.Domain.Statistics;

namespace StatisticsService.Infrastructure.Persistence.Repositories;

public interface IStatisticsProjectionRepository
{
    Task<UserStatistics> GetOrCreateUserStatisticsAsync(
        Guid userId,
        DateTime updatedAtUtc,
        CancellationToken cancellationToken = default);

    Task<UserStatistics?> GetUserStatisticsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<TopicStatistics?> GetTopicStatisticsAsync(Guid userId, string topic, CancellationToken cancellationToken = default);

    Task<DailyStatistics?> GetDailyStatisticsAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DateOnly>> GetStudyDatesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(UserStatistics userStatistics, CancellationToken cancellationToken = default);

    Task AddAsync(TopicStatistics topicStatistics, CancellationToken cancellationToken = default);

    Task AddAsync(DailyStatistics dailyStatistics, CancellationToken cancellationToken = default);
}
