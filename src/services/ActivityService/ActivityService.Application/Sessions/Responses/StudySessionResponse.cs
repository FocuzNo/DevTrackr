using ActivityService.Domain.Sessions;

namespace ActivityService.Application.Sessions.Responses;

public sealed record StudySessionResponse(
    Guid Id,
    Guid UserId,
    Guid GoalId,
    string Topic,
    int DurationMinutes,
    StudySessionDifficulty Difficulty,
    string? Note,
    DateOnly SessionDate,
    DateTime CreatedAt,
    DateTime UpdatedAt);
