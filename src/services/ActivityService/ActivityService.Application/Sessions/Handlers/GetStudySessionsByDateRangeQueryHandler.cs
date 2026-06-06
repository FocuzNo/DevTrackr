using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Application.Sessions.Queries;
using ActivityService.Application.Sessions.Responses;
using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;

namespace ActivityService.Application.Sessions.Handlers;

public sealed class GetStudySessionsByDateRangeQueryHandler(
    IStudySessionRepository studySessionRepository)
    : IQueryHandler<GetStudySessionsByDateRangeQuery, Result<IReadOnlyList<StudySessionListItemResponse>>>
{
    public async Task<Result<IReadOnlyList<StudySessionListItemResponse>>> HandleAsync(
        GetStudySessionsByDateRangeQuery query,
        CancellationToken cancellationToken = default)
    {
        var studySessions = await studySessionRepository.GetByDateRangeAsync(query.UserId, query.From, query.To, cancellationToken);
        var response = studySessions.Select(x => x.ToListItemResponse()).ToArray();
        return Result<IReadOnlyList<StudySessionListItemResponse>>.Success(response);
    }
}
