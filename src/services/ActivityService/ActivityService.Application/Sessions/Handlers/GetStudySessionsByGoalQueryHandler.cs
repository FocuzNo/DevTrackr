using ActivityService.Application.Abstractions;
using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Application.Sessions.Queries;
using ActivityService.Application.Sessions.Responses;

namespace ActivityService.Application.Sessions.Handlers;

public sealed class GetStudySessionsByGoalQueryHandler(IStudySessionRepository studySessionRepository)
    : IQueryHandler<GetStudySessionsByGoalQuery, IReadOnlyList<StudySessionListItemResponse>>
{
    public async Task<IReadOnlyList<StudySessionListItemResponse>> HandleAsync(GetStudySessionsByGoalQuery query, CancellationToken cancellationToken = default)
    {
        var studySessions = await studySessionRepository.GetByGoalIdAsync(query.UserId, query.GoalId, cancellationToken);
        return studySessions.Select(x => x.ToListItemResponse()).ToArray();
    }
}
