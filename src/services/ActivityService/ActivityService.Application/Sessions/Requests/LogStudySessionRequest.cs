using ActivityService.Domain.Sessions;

namespace ActivityService.Application.Sessions.Requests;

public sealed record LogStudySessionRequest(
    Guid GoalId,
    string Topic,
    int DurationMinutes,
    StudySessionDifficulty Difficulty,
    string? Note,
    DateOnly SessionDate);
