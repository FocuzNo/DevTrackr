namespace ActivityService.Application.Sessions.Queries;

public sealed record GetStudySessionsByGoalQuery(Guid UserId, Guid GoalId);
