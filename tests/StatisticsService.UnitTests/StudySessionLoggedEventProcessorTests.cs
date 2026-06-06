using DevTrackr.Contracts;
using Microsoft.Extensions.Logging.Abstractions;
using StatisticsService.Application.Abstractions.Caching;
using StatisticsService.Application.Statistics.Responses;
using StatisticsService.Domain.Statistics;
using StatisticsService.Infrastructure.Messaging;
using StatisticsService.Infrastructure.Persistence;
using StatisticsService.Infrastructure.Persistence.Entities;
using StatisticsService.Infrastructure.Persistence.Repositories;
using Xunit;

namespace StatisticsService.UnitTests;

public sealed class StudySessionLoggedEventProcessorTests
{
    [Fact]
    public async Task ProcessAsync_UpdatesStatistics_AndInvalidatesCache()
    {
        var repository = new InMemoryProjectionRepository();
        var processedEvents = new InMemoryProcessedIntegrationEventRepository();
        var cache = new TestDashboardCache();
        var processor = new StudySessionLoggedEventProcessor(
            repository,
            processedEvents,
            new TestUnitOfWork(),
            cache,
            NullLogger<StudySessionLoggedEventProcessor>.Instance);

        var integrationEvent = new StudySessionLoggedIntegrationEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            "Architecture",
            45,
            4,
            new DateOnly(2026, 6, 6),
            DateTime.UtcNow);

        await processor.ProcessAsync(integrationEvent, CancellationToken.None);

        Assert.Single(repository.UserStatisticsItems);
        Assert.Single(repository.TopicStatisticsItems);
        Assert.Single(repository.DailyStatisticsItems);
        Assert.Equal(1, cache.Invalidations);
    }

    [Fact]
    public async Task ProcessAsync_DuplicateEvent_IsIgnored()
    {
        var eventId = Guid.NewGuid();
        var processedEvents = new InMemoryProcessedIntegrationEventRepository(eventId);
        var repository = new InMemoryProjectionRepository();
        var cache = new TestDashboardCache();
        var processor = new StudySessionLoggedEventProcessor(
            repository,
            processedEvents,
            new TestUnitOfWork(),
            cache,
            NullLogger<StudySessionLoggedEventProcessor>.Instance);

        await processor.ProcessAsync(
            new StudySessionLoggedIntegrationEvent(
                eventId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "DotNet",
                30,
                3,
                new DateOnly(2026, 6, 6),
                DateTime.UtcNow),
            CancellationToken.None);

        Assert.Empty(repository.UserStatisticsItems);
        Assert.Equal(0, cache.Invalidations);
    }

    [Fact]
    public void RecalculateStreaks_ComputesCurrentAndLongest()
    {
        var result = StudySessionLoggedEventProcessor.RecalculateStreaks(
            [new DateOnly(2026, 6, 5), new DateOnly(2026, 6, 4), new DateOnly(2026, 6, 2)],
            new DateOnly(2026, 6, 6));

        Assert.Equal(3, result.currentStreak);
        Assert.Equal(3, result.longestStreak);
    }

    private sealed class InMemoryProjectionRepository : IStatisticsProjectionRepository
    {
        public List<UserStatistics> UserStatisticsItems { get; } = [];
        public List<TopicStatistics> TopicStatisticsItems { get; } = [];
        public List<DailyStatistics> DailyStatisticsItems { get; } = [];

        public Task<UserStatistics?> GetUserStatisticsAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult(UserStatisticsItems.FirstOrDefault(x => x.UserId == userId));

        public async Task<UserStatistics> GetOrCreateUserStatisticsAsync(
            Guid userId,
            DateTime updatedAtUtc,
            CancellationToken cancellationToken = default)
        {
            var userStatistics = await GetUserStatisticsAsync(userId, cancellationToken);
            if (userStatistics is not null)
            {
                return userStatistics;
            }

            userStatistics = StatisticsService.Domain.Statistics.UserStatistics.Create(userId, updatedAtUtc);
            UserStatisticsItems.Add(userStatistics);

            return userStatistics;
        }

        public Task<TopicStatistics?> GetTopicStatisticsAsync(Guid userId, string topic, CancellationToken cancellationToken = default) =>
            Task.FromResult(TopicStatisticsItems.FirstOrDefault(x => x.UserId == userId && x.Topic == topic));

        public Task<DailyStatistics?> GetDailyStatisticsAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default) =>
            Task.FromResult(DailyStatisticsItems.FirstOrDefault(x => x.UserId == userId && x.Date == date));

        public Task<IReadOnlyList<DateOnly>> GetStudyDatesAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<DateOnly>>(DailyStatisticsItems.Where(x => x.UserId == userId).Select(x => x.Date).OrderByDescending(x => x).ToArray());

        public Task AddAsync(UserStatistics userStatistics, CancellationToken cancellationToken = default)
        {
            UserStatisticsItems.Add(userStatistics);
            return Task.CompletedTask;
        }

        public Task AddAsync(TopicStatistics topicStatistics, CancellationToken cancellationToken = default)
        {
            TopicStatisticsItems.Add(topicStatistics);
            return Task.CompletedTask;
        }

        public Task AddAsync(DailyStatistics dailyStatistics, CancellationToken cancellationToken = default)
        {
            DailyStatisticsItems.Add(dailyStatistics);
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryProcessedIntegrationEventRepository(params Guid[] existingEventIds) : IProcessedIntegrationEventRepository
    {
        private readonly HashSet<Guid> _eventIds = [.. existingEventIds];

        public Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken = default) =>
            Task.FromResult(_eventIds.Contains(eventId));

        public Task AddAsync(ProcessedIntegrationEvent processedEvent, CancellationToken cancellationToken = default)
        {
            _eventIds.Add(processedEvent.EventId);
            return Task.CompletedTask;
        }
    }

    private sealed class TestUnitOfWork : IStatisticsUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }

    private sealed class TestDashboardCache : IDashboardCache
    {
        public int Invalidations { get; private set; }

        public Task<DashboardResponse?> GetAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult<DashboardResponse?>(null);

        public Task SetAsync(Guid userId, DashboardResponse dashboard, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task InvalidateAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            Invalidations++;
            return Task.CompletedTask;
        }
    }
}
