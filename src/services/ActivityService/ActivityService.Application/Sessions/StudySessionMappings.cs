using ActivityService.Application.Sessions.Responses;
using ActivityService.Domain.Sessions;

namespace ActivityService.Application.Sessions;

internal static class StudySessionMappings
{
    public static StudySessionResponse ToResponse(this StudySession studySession) =>
        new(
            studySession.Id,
            studySession.UserId,
            studySession.GoalId,
            studySession.Topic,
            studySession.DurationMinutes,
            studySession.Difficulty,
            studySession.Note,
            studySession.SessionDate,
            studySession.CreatedAt,
            studySession.UpdatedAt);

    public static StudySessionListItemResponse ToListItemResponse(this StudySession studySession) =>
        new(
            studySession.Id,
            studySession.GoalId,
            studySession.Topic,
            studySession.DurationMinutes,
            studySession.Difficulty,
            studySession.SessionDate,
            studySession.UpdatedAt);
}
