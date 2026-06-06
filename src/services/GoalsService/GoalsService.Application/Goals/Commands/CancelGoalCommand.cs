namespace GoalsService.Application.Goals.Commands;

public sealed record CancelGoalCommand(Guid UserId, Guid GoalId);
