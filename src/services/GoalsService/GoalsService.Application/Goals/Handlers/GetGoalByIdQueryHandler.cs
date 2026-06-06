using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Application.Goals.Queries;
using GoalsService.Application.Goals.Responses;
using GoalsService.Domain.Goals;
namespace GoalsService.Application.Goals.Handlers;

public sealed class GetGoalByIdQueryHandler(IGoalReadRepository goalReadRepository)
    : IQueryHandler<GetGoalByIdQuery, Result<GoalResponse>>
{
    public async Task<Result<GoalResponse>> HandleAsync(
        GetGoalByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var goal = await goalReadRepository.GetByIdAsync(query.GoalId, query.UserId, cancellationToken);
        return goal is null
            ? Result<GoalResponse>.Failure(GoalErrors.GoalNotFound)
            : Result<GoalResponse>.Success(goal);
    }
}
