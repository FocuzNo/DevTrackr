using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Application.Sessions.Queries;
using ActivityService.Application.Sessions.Responses;
using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;

namespace ActivityService.Application.Sessions.Handlers;

public sealed class GetStudySessionsByDateRangeQueryHandler(
    IStudySessionReadRepository studySessionReadRepository)
    : IQueryHandler<GetStudySessionsByDateRangeQuery, Result<IReadOnlyList<StudySessionListItemResponse>>>
{
    public async Task<Result<IReadOnlyList<StudySessionListItemResponse>>> HandleAsync(
        GetStudySessionsByDateRangeQuery query,
        CancellationToken cancellationToken = default)
    {
        var studySessions = await studySessionReadRepository.GetByDateRangeAsync(
            query.UserId,
            query.From,
            query.To,
            cancellationToken);

        return Result<IReadOnlyList<StudySessionListItemResponse>>.Success(studySessions);
    }
}
