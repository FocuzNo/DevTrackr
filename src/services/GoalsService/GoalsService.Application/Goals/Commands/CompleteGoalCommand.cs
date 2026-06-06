using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using GoalsService.Application.Goals.Responses;

namespace GoalsService.Application.Goals.Commands;

public sealed record CompleteGoalCommand(Guid UserId, Guid GoalId) : ICommand<Result<GoalResponse>>;
