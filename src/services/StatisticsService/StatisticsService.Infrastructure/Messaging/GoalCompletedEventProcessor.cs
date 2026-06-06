using DevTrackr.Contracts;
using Microsoft.Extensions.Logging;
using StatisticsService.Application.Abstractions.Caching;
using StatisticsService.Domain.Statistics;
using StatisticsService.Infrastructure.Persistence;
using StatisticsService.Infrastructure.Persistence.Entities;
using StatisticsService.Infrastructure.Persistence.Repositories;

namespace StatisticsService.Infrastructure.Messaging;

public sealed class GoalCompletedEventProcessor(
    IStatisticsProjectionRepository repository,
    IProcessedIntegrationEventRepository processedEvents,
    IStatisticsUnitOfWork unitOfWork,
    IDashboardCache cache,
    ILogger<GoalCompletedEventProcessor> logger)
{
    public async Task ProcessAsync(GoalCompletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (await processedEvents.ExistsAsync(integrationEvent.EventId, cancellationToken))
        {
            logger.LogInformation(
                "Goal completion event {EventId} already processed for UserId {UserId}",
                integrationEvent.EventId,
                integrationEvent.UserId);
            return;
        }

        var userStatistics = await repository.GetUserStatisticsAsync(integrationEvent.UserId, cancellationToken)
            ?? UserStatistics.Create(integrationEvent.UserId, integrationEvent.OccurredOnUtc);

        if (userStatistics.TotalSessions == 0 && userStatistics.TotalStudyMinutes == 0)
        {
            var existing = await repository.GetUserStatisticsAsync(integrationEvent.UserId, cancellationToken);
            if (existing is null)
            {
                await repository.AddAsync(userStatistics, cancellationToken);
            }
            else
            {
                userStatistics = existing;
            }
        }

        userStatistics.ApplyGoalCompleted(integrationEvent.OccurredOnUtc);

        await processedEvents.AddAsync(
            new ProcessedIntegrationEvent
            {
                EventId = integrationEvent.EventId,
                EventType = nameof(GoalCompletedIntegrationEvent),
                ProcessedAtUtc = DateTime.UtcNow
            },
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cache.InvalidateAsync(integrationEvent.UserId, cancellationToken);
    }
}
