namespace GoalsService.Application.Goals.Queries;

public sealed record GetGoalByIdQuery(Guid UserId, Guid GoalId);
