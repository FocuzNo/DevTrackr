namespace GoalsService.Application.Goals.Commands;

public sealed record AddGoalProgressCommand(Guid UserId, Guid GoalId, int MinutesToAdd);
