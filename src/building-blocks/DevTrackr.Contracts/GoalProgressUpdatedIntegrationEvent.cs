namespace DevTrackr.Contracts;

public sealed record GoalProgressUpdatedIntegrationEvent(
    Guid EventId,
    Guid GoalId,
    Guid UserId,
    int CurrentMinutes,
    int TargetMinutes,
    decimal ProgressPercentage,
    DateTime OccurredOnUtc);
