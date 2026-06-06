using DevTrackr.Contracts;
using GoalsService.Application.Abstractions.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GoalsService.Infrastructure.Messaging;

public sealed class StudySessionLoggedIntegrationEventProcessor(
    IGoalRepository goalRepository,
    IProcessedIntegrationEventRepository processedIntegrationEventRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint,
    ILogger<StudySessionLoggedIntegrationEventProcessor> logger)
{
    public async Task ProcessAsync(StudySessionLoggedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        var eventId = integrationEvent.EventId;
        if (await processedIntegrationEventRepository.ExistsAsync(eventId, cancellationToken))
        {
            logger.LogInformation(
                "StudySessionLoggedIntegrationEvent with EventId {EventId} was already processed for GoalId {GoalId}.",
                eventId,
                integrationEvent.GoalId);
            return;
        }

        var goal = await goalRepository.GetByIdAsync(integrationEvent.GoalId, integrationEvent.UserId, cancellationToken);
        if (goal is null)
        {
            logger.LogWarning(
                "Goal {GoalId} for user {UserId} was not found while processing StudySessionLoggedIntegrationEvent {EventId}. Marking as processed.",
                integrationEvent.GoalId,
                integrationEvent.UserId,
                eventId);

            await processedIntegrationEventRepository.AddAsync(
                eventId,
                nameof(StudySessionLoggedIntegrationEvent),
                DateTime.UtcNow,
                cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var result = goal.AddProgress(integrationEvent.DurationMinutes, DateTime.UtcNow);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Progress update skipped for GoalId {GoalId} due to rule violation: {ErrorCode} - {ErrorMessage}",
                integrationEvent.GoalId,
                result.Error.Code,
                result.Error.Message);
        }
        else
        {
            await publishEndpoint.Publish(
                new GoalProgressUpdatedIntegrationEvent(
                    EventId: Guid.NewGuid(),
                    GoalId: goal.Id,
                    UserId: goal.UserId,
                    CurrentMinutes: goal.CurrentMinutes,
                    TargetMinutes: goal.TargetMinutes,
                    ProgressPercentage: goal.ProgressPercentage,
                    OccurredOnUtc: DateTime.UtcNow),
                cancellationToken);
        }

        await processedIntegrationEventRepository.AddAsync(
            eventId,
            nameof(StudySessionLoggedIntegrationEvent),
            DateTime.UtcNow,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
