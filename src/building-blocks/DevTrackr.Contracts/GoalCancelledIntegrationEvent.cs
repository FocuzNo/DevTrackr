namespace DevTrackr.Contracts;

public sealed record GoalCancelledIntegrationEvent(
    Guid EventId,
    Guid GoalId,
    Guid UserId,
    DateTime OccurredAt);
