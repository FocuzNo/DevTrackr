namespace ActivityService.Application.Sessions;

public sealed record LogStudySessionRequest(Guid UserId, Guid GoalId, string Topic, int DurationMinutes, DateOnly SessionDate);
