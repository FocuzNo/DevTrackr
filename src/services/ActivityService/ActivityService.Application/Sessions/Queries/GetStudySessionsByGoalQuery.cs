using ActivityService.Application.Sessions.Responses;
using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;

namespace ActivityService.Application.Sessions.Queries;

public sealed record GetStudySessionsByGoalQuery(Guid UserId, Guid GoalId) : IQuery<Result<IReadOnlyList<StudySessionListItemResponse>>>;
