using DevTrackr.Contracts;
using Microsoft.Extensions.Logging;
using StatisticsService.Application.Abstractions.Caching;
using StatisticsService.Domain.Statistics;
using StatisticsService.Infrastructure.Persistence;
using StatisticsService.Infrastructure.Persistence.Entities;
using StatisticsService.Infrastructure.Persistence.Repositories;

namespace StatisticsService.Infrastructure.Messaging;

public sealed class GoalCreatedEventProcessor(
    IStatisticsProjectionRepository repository,
    IProcessedIntegrationEventRepository processedEvents,
    IStatisticsUnitOfWork unitOfWork,
    IDashboardCache cache,
    ILogger<GoalCreatedEventProcessor> logger)
{
    public async Task ProcessAsync(
        GoalCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        if (await processedEvents.ExistsAsync(integrationEvent.EventId, cancellationToken))
        {
            logger.LogInformation(
                "Goal created event {EventId} already processed for UserId {UserId}",
                integrationEvent.EventId,
                integrationEvent.UserId);
            return;
        }

        var userStatistics = await repository.GetOrCreateUserStatisticsAsync(
            integrationEvent.UserId,
            integrationEvent.OccurredAt,
            cancellationToken);

        userStatistics.ApplyGoalCreated(integrationEvent.OccurredAt);

        await processedEvents.AddAsync(
            new ProcessedIntegrationEvent
            {
                EventId = integrationEvent.EventId,
                EventType = nameof(GoalCreatedIntegrationEvent),
                ProcessedAtUtc = DateTime.UtcNow
            },
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cache.InvalidateAsync(integrationEvent.UserId, cancellationToken);
    }
}
