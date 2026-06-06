using DevTrackr.Cqrs.Abstractions;
using DevTrackr.Contracts;
using GoalsService.Application.Goals.Commands;
using GoalsService.Application.Abstractions.Persistence;
using Microsoft.Extensions.Logging;

namespace GoalsService.Infrastructure.Messaging;

public sealed class StudySessionLoggedIntegrationEventProcessor(
    IAppMediator mediator,
    IProcessedIntegrationEventRepository processedIntegrationEventRepository,
    IUnitOfWork unitOfWork,
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

        var result = await mediator.SendAsync(
            new AddGoalProgressCommand(
                integrationEvent.UserId,
                integrationEvent.GoalId,
                integrationEvent.DurationMinutes),
            cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Progress update skipped for GoalId {GoalId} due to rule violation: {ErrorCode} - {ErrorMessage}",
                integrationEvent.GoalId,
                result.Error.Code,
                result.Error.Message);
        }
        await processedIntegrationEventRepository.AddAsync(
            eventId,
            nameof(StudySessionLoggedIntegrationEvent),
            DateTime.UtcNow,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
