using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Application.Sessions.Queries;
using ActivityService.Application.Sessions.Responses;
using ActivityService.Domain.Sessions;
using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;

namespace ActivityService.Application.Sessions.Handlers;

public sealed class GetStudySessionByIdQueryHandler(IStudySessionRepository studySessionRepository)
    : IQueryHandler<GetStudySessionByIdQuery, Result<StudySessionResponse>>
{
    public async Task<Result<StudySessionResponse>> HandleAsync(GetStudySessionByIdQuery query, CancellationToken cancellationToken = default)
    {
        var studySession = await studySessionRepository.GetByIdAsync(query.SessionId, query.UserId, cancellationToken);
        if (studySession is null)
        {
            return Result<StudySessionResponse>.Failure(StudySessionErrors.StudySessionNotFound);
        }

        return Result<StudySessionResponse>.Success(studySession.ToResponse());
    }
}
