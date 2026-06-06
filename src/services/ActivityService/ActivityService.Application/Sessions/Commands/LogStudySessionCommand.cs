using ActivityService.Domain.Sessions;

namespace ActivityService.Application.Sessions.Commands;

public sealed record LogStudySessionCommand(
    Guid UserId,
    Guid GoalId,
    string Topic,
    int DurationMinutes,
    StudySessionDifficulty Difficulty,
    string? Note,
    DateOnly SessionDate);
