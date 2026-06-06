namespace ActivityService.Application.Sessions.Queries;

public sealed record GetStudySessionsByDateRangeQuery(Guid UserId, DateOnly From, DateOnly To);
