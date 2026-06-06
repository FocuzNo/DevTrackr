using ActivityService.Application.Sessions.Responses;
using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;

namespace ActivityService.Application.Sessions.Queries;

public sealed record GetStudySessionsByDateRangeQuery(Guid UserId, DateOnly From, DateOnly To) : IQuery<Result<IReadOnlyList<StudySessionListItemResponse>>>;
