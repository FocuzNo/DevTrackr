using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using GoalsService.Application.Goals.Responses;

namespace GoalsService.Application.Goals.Queries;

public sealed record GetGoalsQuery(Guid UserId) : IQuery<Result<IReadOnlyList<GoalListItemResponse>>>;
