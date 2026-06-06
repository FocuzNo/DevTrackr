using DevTrackr.Contracts;
using Microsoft.Extensions.Logging;
using StatisticsService.Application.Abstractions.Caching;
using StatisticsService.Domain.Statistics;
using StatisticsService.Infrastructure.Persistence;
using StatisticsService.Infrastructure.Persistence.Entities;
using StatisticsService.Infrastructure.Persistence.Repositories;

namespace StatisticsService.Infrastructure.Messaging;

public sealed class GoalProgressUpdatedEventProcessor(
    IStatisticsProjectionRepository repository,
    IProcessedIntegrationEventRepository processedEvents,
    IStatisticsUnitOfWork unitOfWork,
    IDashboardCache cache,
    ILogger<GoalProgressUpdatedEventProcessor> logger)
{
    public async Task ProcessAsync(GoalProgressUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (await processedEvents.ExistsAsync(integrationEvent.EventId, cancellationToken))
        {
            logger.LogInformation(
                "Goal progress event {EventId} already processed for UserId {UserId}",
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

        userStatistics.ApplyGoalProgress(integrationEvent.OccurredOnUtc);

        await processedEvents.AddAsync(
            new ProcessedIntegrationEvent
            {
                EventId = integrationEvent.EventId,
                EventType = nameof(GoalProgressUpdatedIntegrationEvent),
                ProcessedAtUtc = DateTime.UtcNow
            },
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cache.InvalidateAsync(integrationEvent.UserId, cancellationToken);
    }
}
