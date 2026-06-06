using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Application.Sessions.Queries;
using ActivityService.Application.Sessions.Responses;
using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;

namespace ActivityService.Application.Sessions.Handlers;

public sealed class GetStudySessionsByGoalQueryHandler(IStudySessionReadRepository studySessionReadRepository)
    : IQueryHandler<GetStudySessionsByGoalQuery, Result<IReadOnlyList<StudySessionListItemResponse>>>
{
    public async Task<Result<IReadOnlyList<StudySessionListItemResponse>>> HandleAsync(
        GetStudySessionsByGoalQuery query,
        CancellationToken cancellationToken = default)
    {
        var studySessions = await studySessionReadRepository.GetByGoalIdAsync(query.UserId, query.GoalId, cancellationToken);
        return Result<IReadOnlyList<StudySessionListItemResponse>>.Success(studySessions);
    }
}
