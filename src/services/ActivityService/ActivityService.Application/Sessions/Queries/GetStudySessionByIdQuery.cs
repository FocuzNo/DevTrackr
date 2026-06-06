using ActivityService.Application.Sessions.Responses;
using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;

namespace ActivityService.Application.Sessions.Queries;

public sealed record GetStudySessionByIdQuery(Guid UserId, Guid SessionId) : IQuery<Result<StudySessionResponse>>;
