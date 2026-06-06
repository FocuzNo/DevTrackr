using DevTrackr.Contracts;
using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Domain.Goals;
using GoalsService.Infrastructure.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;
using Xunit;

namespace GoalsService.UnitTests;

public sealed class StudySessionLoggedIntegrationEventProcessorTests
{
    [Fact]
    public async Task ProcessAsync_IncreasesGoalProgress()
    {
        var goal = Goal.Create(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Master RabbitMQ",
            null,
            GoalCategory.Architecture,
            300,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30),
            DateTime.UtcNow).Value!;

        var goalRepository = new InMemoryGoalRepository(goal);
        var processedRepository = new InMemoryProcessedIntegrationEventRepository();
        var publishEndpoint = new TestPublishEndpoint();
        var processor = new StudySessionLoggedIntegrationEventProcessor(
            goalRepository,
            processedRepository,
            new TestUnitOfWork(),
            publishEndpoint,
            new TestLogger<StudySessionLoggedIntegrationEventProcessor>());

        await processor.ProcessAsync(new StudySessionLoggedIntegrationEvent(
            EventId: Guid.NewGuid(),
            SessionId: Guid.NewGuid(),
            UserId: goal.UserId,
            GoalId: goal.Id,
            Topic: "Queues",
            DurationMinutes: 90,
            SessionDate: new DateOnly(2026, 6, 5),
            OccurredOnUtc: DateTime.UtcNow));

        Assert.Equal(90, goal.CurrentMinutes);
        Assert.Single(publishEndpoint.PublishedMessages.OfType<GoalProgressUpdatedIntegrationEvent>());
    }

    [Fact]
    public async Task ProcessAsync_DuplicateEvent_IsIgnored()
    {
        var goal = Goal.Create(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Master RabbitMQ",
            null,
            GoalCategory.Architecture,
            300,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30),
            DateTime.UtcNow).Value!;

        var eventId = Guid.NewGuid();
        var goalRepository = new InMemoryGoalRepository(goal);
        var processedRepository = new InMemoryProcessedIntegrationEventRepository(eventId);
        var publishEndpoint = new TestPublishEndpoint();
        var processor = new StudySessionLoggedIntegrationEventProcessor(
            goalRepository,
            processedRepository,
            new TestUnitOfWork(),
            publishEndpoint,
            new TestLogger<StudySessionLoggedIntegrationEventProcessor>());

        await processor.ProcessAsync(new StudySessionLoggedIntegrationEvent(
            EventId: eventId,
            SessionId: Guid.NewGuid(),
            UserId: goal.UserId,
            GoalId: goal.Id,
            Topic: "Queues",
            DurationMinutes: 90,
            SessionDate: new DateOnly(2026, 6, 5),
            OccurredOnUtc: DateTime.UtcNow));

        Assert.Equal(0, goal.CurrentMinutes);
        Assert.Empty(publishEndpoint.PublishedMessages);
    }

    private sealed class InMemoryGoalRepository(params Goal[] goals) : IGoalRepository
    {
        private readonly List<Goal> _goals = goals.ToList();

        public Task AddAsync(Goal goal, CancellationToken cancellationToken = default)
        {
            _goals.Add(goal);
            return Task.CompletedTask;
        }

        public Task<Goal?> GetByIdAsync(Guid goalId, Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult(_goals.FirstOrDefault(x => x.Id == goalId && x.UserId == userId));

        public Task<IReadOnlyList<Goal>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Goal>>(_goals.Where(x => x.UserId == userId).ToArray());
    }

    private sealed class InMemoryProcessedIntegrationEventRepository(params Guid[] processedEventIds)
        : IProcessedIntegrationEventRepository
    {
        private readonly HashSet<Guid> _processed = processedEventIds.ToHashSet();

        public Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken = default) =>
            Task.FromResult(_processed.Contains(eventId));

        public Task AddAsync(Guid eventId, string eventType, DateTime processedAtUtc, CancellationToken cancellationToken = default)
        {
            _processed.Add(eventId);
            return Task.CompletedTask;
        }
    }

    private sealed class TestUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }
    }

    private sealed class TestPublishEndpoint : IPublishEndpoint
    {
        public List<object> PublishedMessages { get; } = [];

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer) => throw new NotSupportedException();

        public Task Publish<T>(T message, CancellationToken cancellationToken = default)
            where T : class
        {
            PublishedMessages.Add(message);
            return Task.CompletedTask;
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class => Publish(message, cancellationToken);

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class => Publish(message, cancellationToken);

        public Task Publish(object message, CancellationToken cancellationToken = default)
        {
            PublishedMessages.Add(message);
            return Task.CompletedTask;
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken = default) =>
            Publish(message, cancellationToken);

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) =>
            Publish(message, cancellationToken);

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) =>
            Publish(message, cancellationToken);

        public Task Publish<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            PublishedMessages.Add(values);
            return Task.CompletedTask;
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class => Publish<T>(values, cancellationToken);

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class => Publish<T>(values, cancellationToken);
    }

    private sealed class TestLogger<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}
