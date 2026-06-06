using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Application.Sessions.Commands;
using ActivityService.Application.Sessions.Handlers;
using ActivityService.Application.Sessions.Validators;
using ActivityService.Domain.Sessions;
using DevTrackr.Contracts;
using FluentValidation;
using MassTransit;
using Xunit;

namespace ActivityService.UnitTests;

public sealed class LogStudySessionCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_CreatesStudySession_AndPublishesIntegrationEvent()
    {
        var repository = new InMemoryStudySessionRepository();
        var publishEndpoint = new TestPublishEndpoint();
        var handler = new LogStudySessionCommandHandler(
            repository,
            new TestUnitOfWork(),
            new LogStudySessionCommandValidator(),
            publishEndpoint);

        var command = new LogStudySessionCommand(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            "Learn MassTransit outbox",
            75,
            StudySessionDifficulty.Medium,
            "Went through the publish pipeline.",
            DateOnly.FromDateTime(DateTime.UtcNow));

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Single(repository.Items);

        var integrationEvent = Assert.Single(publishEndpoint.PublishedMessages.OfType<StudySessionLoggedIntegrationEvent>());
        Assert.Equal(command.UserId, integrationEvent.UserId);
        Assert.Equal(command.GoalId, integrationEvent.GoalId);
        Assert.Equal(command.DurationMinutes, integrationEvent.DurationMinutes);
        Assert.Equal((int)command.Difficulty, integrationEvent.Difficulty);
    }

    private sealed class InMemoryStudySessionRepository : IStudySessionRepository
    {
        public List<StudySession> Items { get; } = [];

        public Task AddAsync(StudySession studySession, CancellationToken cancellationToken = default)
        {
            Items.Add(studySession);
            return Task.CompletedTask;
        }

        public Task<StudySession?> GetByIdAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.FirstOrDefault(x => x.Id == sessionId && x.UserId == userId));

        public Task<IReadOnlyList<StudySession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<StudySession>>(Items.Where(x => x.UserId == userId).ToArray());

        public Task<IReadOnlyList<StudySession>> GetByGoalIdAsync(Guid userId, Guid goalId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<StudySession>>(Items.Where(x => x.UserId == userId && x.GoalId == goalId).ToArray());

        public Task<IReadOnlyList<StudySession>> GetByDateRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<StudySession>>(Items.Where(x => x.UserId == userId && x.SessionDate >= from && x.SessionDate <= to).ToArray());
    }

    private sealed class TestUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
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
}
