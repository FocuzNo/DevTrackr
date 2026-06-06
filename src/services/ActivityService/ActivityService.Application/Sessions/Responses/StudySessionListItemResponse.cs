using ActivityService.Domain.Sessions;

namespace ActivityService.Application.Sessions.Responses;

public sealed record StudySessionListItemResponse(
    Guid Id,
    Guid GoalId,
    string Topic,
    int DurationMinutes,
    StudySessionDifficulty Difficulty,
    DateOnly SessionDate,
    DateTime UpdatedAt);
