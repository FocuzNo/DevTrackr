using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Application.Sessions.Queries;
using ActivityService.Application.Sessions.Responses;
using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;

namespace ActivityService.Application.Sessions.Handlers;

public sealed class GetStudySessionsQueryHandler(IStudySessionRepository studySessionRepository)
    : IQueryHandler<GetStudySessionsQuery, Result<IReadOnlyList<StudySessionListItemResponse>>>
{
    public async Task<Result<IReadOnlyList<StudySessionListItemResponse>>> HandleAsync(GetStudySessionsQuery query, CancellationToken cancellationToken = default)
    {
        var studySessions = await studySessionRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        return Result<IReadOnlyList<StudySessionListItemResponse>>.Success(
            studySessions.Select(x => x.ToListItemResponse()).ToArray());
    }
}
