namespace ActivityService.Application.Sessions.Queries;

public sealed record GetStudySessionByIdQuery(Guid UserId, Guid SessionId);
