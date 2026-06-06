namespace DevTrackr.Contracts;

public sealed record GoalCompletedIntegrationEvent(
    Guid EventId,
    Guid GoalId,
    Guid UserId,
    int CurrentMinutes,
    DateTime CompletedAtUtc,
    DateTime OccurredOnUtc);
