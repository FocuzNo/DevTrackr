using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using GoalsService.Application.Abstractions;
using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Application.Goals.Queries;
using GoalsService.Application.Goals.Responses;

namespace GoalsService.Application.Goals.Handlers;

public sealed class GetGoalsQueryHandler(IGoalRepository goalRepository)
    : IQueryHandler<GetGoalsQuery, Result<IReadOnlyList<GoalListItemResponse>>>
{
    public async Task<Result<IReadOnlyList<GoalListItemResponse>>> HandleAsync(GetGoalsQuery query, CancellationToken cancellationToken = default)
    {
        var goals = await goalRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        return Result<IReadOnlyList<GoalListItemResponse>>.Success(goals.Select(x => x.ToListItem()).ToArray());
    }
}
