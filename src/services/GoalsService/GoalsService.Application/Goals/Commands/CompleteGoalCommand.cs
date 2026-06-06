namespace GoalsService.Application.Goals.Commands;

public sealed record CompleteGoalCommand(Guid UserId, Guid GoalId);
