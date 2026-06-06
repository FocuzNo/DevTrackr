using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using GoalsService.Application.Goals.Responses;

namespace GoalsService.Application.Goals.Queries;

public sealed record GetGoalByIdQuery(Guid UserId, Guid GoalId) : IQuery<Result<GoalResponse>>;
