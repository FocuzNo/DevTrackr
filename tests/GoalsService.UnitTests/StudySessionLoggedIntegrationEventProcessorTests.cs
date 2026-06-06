using DevTrackr.Cqrs.Abstractions;
using DevTrackr.Contracts;
using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Application.Goals.Commands;
using GoalsService.Domain.Goals;
using GoalsService.Infrastructure.Messaging;
using DevTrackr.SharedKernel.Primitives;
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

        var processedRepository = new InMemoryProcessedIntegrationEventRepository();
        var mediator = new TestMediator(goal);
        var processor = new StudySessionLoggedIntegrationEventProcessor(
            mediator,
            processedRepository,
            new TestUnitOfWork(),
            new TestLogger<StudySessionLoggedIntegrationEventProcessor>());

        await processor.ProcessAsync(new StudySessionLoggedIntegrationEvent(
            EventId: Guid.NewGuid(),
            SessionId: Guid.NewGuid(),
            UserId: goal.UserId,
            GoalId: goal.Id,
            Topic: "Queues",
            DurationMinutes: 90,
            Difficulty: 3,
            SessionDate: new DateOnly(2026, 6, 5),
            OccurredAt: DateTime.UtcNow));

        Assert.Equal(90, goal.CurrentMinutes);
        Assert.Equal(1, mediator.SendCallCount);
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
        var processedRepository = new InMemoryProcessedIntegrationEventRepository(eventId);
        var mediator = new TestMediator(goal);
        var processor = new StudySessionLoggedIntegrationEventProcessor(
            mediator,
            processedRepository,
            new TestUnitOfWork(),
            new TestLogger<StudySessionLoggedIntegrationEventProcessor>());

        await processor.ProcessAsync(new StudySessionLoggedIntegrationEvent(
            EventId: eventId,
            SessionId: Guid.NewGuid(),
            UserId: goal.UserId,
            GoalId: goal.Id,
            Topic: "Queues",
            DurationMinutes: 90,
            Difficulty: 3,
            SessionDate: new DateOnly(2026, 6, 5),
            OccurredAt: DateTime.UtcNow));

        Assert.Equal(0, goal.CurrentMinutes);
        Assert.Equal(0, mediator.SendCallCount);
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

    private sealed class TestMediator(Goal goal) : IAppMediator
    {
        public int SendCallCount { get; private set; }

        public Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            SendCallCount++;

            if (command is AddGoalProgressCommand addGoalProgressCommand)
            {
                return Task.FromResult(goal.AddProgress(addGoalProgressCommand.MinutesToAdd, DateTime.UtcNow));
            }

            throw new NotSupportedException($"Unsupported command type: {command.GetType().Name}");
        }

        public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<TResult> SendAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
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
