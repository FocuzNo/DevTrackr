using DevTrackr.SharedKernel.Primitives;

namespace GoalsService.Domain.Goals;

public static class GoalErrors
{
    public static readonly Error TitleRequired = Error.Validation("Goals.TitleRequired", "Title is required.");
    public static readonly Error TitleTooLong = Error.Validation("Goals.TitleTooLong", "Title cannot exceed 100 characters.");
    public static readonly Error DescriptionTooLong = Error.Validation("Goals.DescriptionTooLong", "Description cannot exceed 1000 characters.");
    public static readonly Error TargetMinutesInvalid = Error.Validation("Goals.TargetMinutesInvalid", "Target minutes must be greater than zero.");
    public static readonly Error ProgressMinutesInvalid = Error.Validation("Goals.ProgressMinutesInvalid", "Progress minutes must be greater than zero.");
    public static readonly Error CurrentMinutesInvalid = Error.Validation("Goals.CurrentMinutesInvalid", "Current minutes cannot be negative.");
    public static readonly Error DeadlineBeforeStartDate = Error.Validation("Goals.DeadlineBeforeStartDate", "Deadline cannot be earlier than start date.");
    public static readonly Error CompletedGoalCannotBeModified = Error.Failure("Goals.CompletedGoalCannotBeModified", "Completed goals cannot be modified.");
    public static readonly Error CancelledGoalCannotBeModified = Error.Failure("Goals.CancelledGoalCannotBeModified", "Cancelled goals cannot be modified.");
    public static readonly Error CompletedGoalCannotReceiveProgress = Error.Failure("Goals.CompletedGoalCannotReceiveProgress", "Completed goals cannot receive progress.");
    public static readonly Error CancelledGoalCannotReceiveProgress = Error.Failure("Goals.CancelledGoalCannotReceiveProgress", "Cancelled goals cannot receive progress.");
    public static readonly Error GoalCannotBeCompletedYet = Error.Failure("Goals.GoalCannotBeCompletedYet", "Goal can be completed only when progress is at least 80%.");
    public static readonly Error GoalNotFound = Error.Failure("Goals.GoalNotFound", "Goal was not found.");
}
