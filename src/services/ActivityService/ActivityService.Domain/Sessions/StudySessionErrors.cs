using DevTrackr.SharedKernel.Primitives;

namespace ActivityService.Domain.Sessions;

public static class StudySessionErrors
{
    public static readonly Error TopicRequired = Error.Validation("Activity.TopicRequired", "Topic is required.");
    public static readonly Error TopicTooLong = Error.Validation("Activity.TopicTooLong", "Topic cannot exceed 100 characters.");
    public static readonly Error NoteTooLong = Error.Validation("Activity.NoteTooLong", "Note cannot exceed 1000 characters.");
    public static readonly Error DurationInvalid = Error.Validation("Activity.DurationInvalid", "Duration must be greater than zero.");
    public static readonly Error DurationTooLong = Error.Validation("Activity.DurationTooLong", "Duration cannot exceed 480 minutes.");
    public static readonly Error DifficultyInvalid = Error.Validation("Activity.DifficultyInvalid", "Difficulty must be between 1 and 5.");
    public static readonly Error SessionDateInFuture = Error.Validation("Activity.SessionDateInFuture", "Session date cannot be in the future.");
    public static readonly Error GoalIdRequired = Error.Validation("Activity.GoalIdRequired", "Goal id is required.");
    public static readonly Error UserIdRequired = Error.Validation("Activity.UserIdRequired", "User id is required.");
    public static readonly Error StudySessionNotFound = Error.Failure("Activity.StudySessionNotFound", "Study session was not found.");
}
