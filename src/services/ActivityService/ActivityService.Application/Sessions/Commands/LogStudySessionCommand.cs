using ActivityService.Application.Sessions.Responses;
using ActivityService.Domain.Sessions;
using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;

namespace ActivityService.Application.Sessions.Commands;

public sealed record LogStudySessionCommand(
    Guid UserId,
    Guid GoalId,
    string Topic,
    int DurationMinutes,
    StudySessionDifficulty Difficulty,
    string? Note,
    DateOnly SessionDate) : ICommand<Result<StudySessionResponse>>;
