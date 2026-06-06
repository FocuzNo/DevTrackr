using ActivityService.Application.Abstractions;
using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Application.Sessions.Queries;
using ActivityService.Application.Sessions.Responses;

namespace ActivityService.Application.Sessions.Handlers;

public sealed class GetStudySessionsQueryHandler(IStudySessionRepository studySessionRepository)
    : IQueryHandler<GetStudySessionsQuery, IReadOnlyList<StudySessionListItemResponse>>
{
    public async Task<IReadOnlyList<StudySessionListItemResponse>> HandleAsync(GetStudySessionsQuery query, CancellationToken cancellationToken = default)
    {
        var studySessions = await studySessionRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        return studySessions.Select(x => x.ToListItemResponse()).ToArray();
    }
}
