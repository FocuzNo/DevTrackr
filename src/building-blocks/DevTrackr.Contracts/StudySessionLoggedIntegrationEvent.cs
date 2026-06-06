namespace DevTrackr.Contracts;

public sealed record StudySessionLoggedIntegrationEvent(
    Guid EventId,
    Guid SessionId,
    Guid UserId,
    Guid GoalId,
    string Topic,
    int DurationMinutes,
    int Difficulty,
    DateOnly SessionDate,
    DateTime OccurredAt);
