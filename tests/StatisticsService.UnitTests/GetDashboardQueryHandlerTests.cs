using DevTrackr.SharedKernel.Primitives;
using StatisticsService.Application.Abstractions.Caching;
using StatisticsService.Application.Abstractions.Persistence;
using StatisticsService.Application.Statistics.Handlers;
using StatisticsService.Application.Statistics.Queries;
using StatisticsService.Application.Statistics.Responses;
using StatisticsService.Domain.Statistics;
using Xunit;

namespace StatisticsService.UnitTests;

public sealed class GetDashboardQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_BuildsDashboard_AndCachesIt()
    {
        var cache = new TestDashboardCache();
        var repository = new TestStatisticsReadRepository(
            UserStatistics.Create(Guid.Parse("11111111-1111-1111-1111-111111111111"), DateTime.UtcNow),
            [
                CreateTopic("Architecture", 120, 2, 4.50m),
                CreateTopic("DotNet", 90, 3, 3.33m)
            ],
            [
                CreateDay(new DateOnly(2026, 6, 1), 30, 1, 3),
                CreateDay(new DateOnly(2026, 6, 2), 45, 1, 4)
            ]);

        repository.UserStatistics.ApplyGoalProgress(DateTime.UtcNow);
        repository.UserStatistics.ApplyGoalCompleted(DateTime.UtcNow);
        repository.UserStatistics.ApplyStudySession(75, 4, new DateOnly(2026, 6, 2), 2, 3, DateTime.UtcNow);

        var handler = new GetDashboardQueryHandler(repository, cache);

        var result = await handler.HandleAsync(new GetDashboardQuery(repository.UserStatistics.UserId));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(75, result.Value!.TotalStudyMinutes);
        Assert.Equal(1, result.Value.TotalSessions);
        Assert.Equal(1, result.Value.CompletedGoals);
        Assert.NotEmpty(result.Value.TopTopics);
        Assert.Equal(1, cache.SetCalls);
    }

    [Fact]
    public async Task HandleAsync_UsesCache_WhenAvailable()
    {
        var cached = new DashboardResponse(10, 1, 0, 1, 1, 1, 3, [], []);
        var cache = new TestDashboardCache(cached);
        var handler = new GetDashboardQueryHandler(new TestStatisticsReadRepository(null, [], []), cache);

        var result = await handler.HandleAsync(new GetDashboardQuery(Guid.NewGuid()));

        Assert.True(result.IsSuccess);
        Assert.Equal(cached, result.Value);
        Assert.Equal(0, cache.SetCalls);
    }

    private static TopicStatistics CreateTopic(string topic, int totalMinutes, int sessionsCount, decimal averageDifficulty)
    {
        var value = TopicStatistics.Create(Guid.NewGuid(), topic, DateTime.UtcNow);
        for (var i = 0; i < sessionsCount; i++)
        {
            value.ApplyStudySession(totalMinutes / sessionsCount, (int)Math.Round(averageDifficulty), DateTime.UtcNow);
        }

        return value;
    }

    private static DailyStatistics CreateDay(DateOnly date, int minutes, int sessions, decimal averageDifficulty)
    {
        var value = DailyStatistics.Create(Guid.NewGuid(), date, DateTime.UtcNow);
        for (var i = 0; i < sessions; i++)
        {
            value.ApplyStudySession(minutes / sessions, (int)Math.Round(averageDifficulty), DateTime.UtcNow);
        }

        return value;
    }

    private sealed class TestDashboardCache(DashboardResponse? response = null) : IDashboardCache
    {
        public int SetCalls { get; private set; }

        public Task<DashboardResponse?> GetAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult(response);

        public Task SetAsync(Guid userId, DashboardResponse dashboard, CancellationToken cancellationToken = default)
        {
            SetCalls++;
            return Task.CompletedTask;
        }

        public Task InvalidateAsync(Guid userId, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class TestStatisticsReadRepository(
        UserStatistics? userStatistics,
        IReadOnlyList<TopicStatistics> topicStatistics,
        IReadOnlyList<DailyStatistics> dailyStatistics) : IStatisticsReadRepository
    {
        public UserStatistics UserStatistics { get; } = userStatistics ?? UserStatistics.Create(Guid.NewGuid(), DateTime.UtcNow);

        public Task<UserStatistics?> GetUserStatisticsAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult(userStatistics);

        public Task<IReadOnlyList<TopicStatistics>> GetTopicStatisticsAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult(topicStatistics);

        public Task<IReadOnlyList<DailyStatistics>> GetDailyStatisticsAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<DailyStatistics>>(dailyStatistics.Where(x => x.Date >= from && x.Date <= to).ToArray());
    }
}
