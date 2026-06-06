namespace DevTrackr.Contracts;

public sealed record GoalCreatedIntegrationEvent(
    Guid EventId,
    Guid GoalId,
    Guid UserId,
    string Title,
    string Category,
    int TargetMinutes,
    DateTime OccurredAt);
