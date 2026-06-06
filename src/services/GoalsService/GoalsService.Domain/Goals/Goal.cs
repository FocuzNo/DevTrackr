using DevTrackr.SharedKernel.Primitives;

namespace GoalsService.Domain.Goals;

public sealed class Goal : AggregateRoot<Guid>
{
    private Goal(
        Guid id,
        Guid userId,
        string title,
        string? description,
        GoalCategory category,
        int targetMinutes,
        int currentMinutes,
        DateOnly startDate,
        DateOnly deadline,
        GoalStatus status,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime? completedAt)
        : base(id)
    {
        UserId = userId;
        Title = title;
        Description = description;
        Category = category;
        TargetMinutes = targetMinutes;
        CurrentMinutes = currentMinutes;
        StartDate = startDate;
        Deadline = deadline;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        CompletedAt = completedAt;
    }

    private Goal()
        : base(Guid.Empty)
    {
    }

    public Guid UserId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public GoalCategory Category { get; private set; }

    public int TargetMinutes { get; private set; }

    public int CurrentMinutes { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly Deadline { get; private set; }

    public GoalStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public decimal ProgressPercentage =>
        TargetMinutes <= 0
            ? 0
            : decimal.Round((decimal)CurrentMinutes / TargetMinutes * 100, 2);

    public static Result<Goal> Create(
        Guid userId,
        string title,
        string? description,
        GoalCategory category,
        int targetMinutes,
        DateOnly startDate,
        DateOnly deadline,
        DateTime utcNow)
    {
        var validationResult = Validate(title, description, targetMinutes, 0, startDate, deadline);
        if (validationResult.IsFailure)
        {
            return Result<Goal>.Failure(validationResult.Error);
        }

        var goal = new Goal(
            Guid.NewGuid(),
            userId,
            title.Trim(),
            Normalize(description),
            category,
            targetMinutes,
            0,
            startDate,
            deadline,
            GoalStatus.Active,
            utcNow,
            utcNow,
            completedAt: null);

        return Result<Goal>.Success(goal);
    }

    public Result UpdateDetails(
        string title,
        string? description,
        GoalCategory category,
        int targetMinutes,
        DateOnly startDate,
        DateOnly deadline,
        DateTime utcNow)
    {
        var stateResult = EnsureCanBeModified();
        if (stateResult.IsFailure)
        {
            return stateResult;
        }

        var validationResult = Validate(title, description, targetMinutes, CurrentMinutes, startDate, deadline);
        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        Title = title.Trim();
        Description = Normalize(description);
        Category = category;
        TargetMinutes = targetMinutes;
        StartDate = startDate;
        Deadline = deadline;
        UpdatedAt = utcNow;
        RecalculateStatus();

        return Result.Success();
    }

    public Result AddProgress(int minutes, DateTime utcNow)
    {
        if (Status == GoalStatus.Cancelled)
        {
            return Result.Failure(GoalErrors.CancelledGoalCannotReceiveProgress);
        }

        if (Status == GoalStatus.Completed)
        {
            return Result.Failure(GoalErrors.CompletedGoalCannotReceiveProgress);
        }

        if (minutes <= 0)
        {
            return Result.Failure(GoalErrors.ProgressMinutesInvalid);
        }

        CurrentMinutes += minutes;
        UpdatedAt = utcNow;
        RecalculateStatus();

        return Result.Success();
    }

    public Result Complete(DateTime utcNow)
    {
        var stateResult = EnsureCanBeModified();
        if (stateResult.IsFailure)
        {
            return stateResult;
        }

        if (ProgressPercentage < 80)
        {
            return Result.Failure(GoalErrors.GoalCannotBeCompletedYet);
        }

        Status = GoalStatus.Completed;
        CompletedAt = utcNow;
        UpdatedAt = utcNow;

        return Result.Success();
    }

    public Result Cancel(DateTime utcNow)
    {
        var stateResult = EnsureCanBeModified();
        if (stateResult.IsFailure)
        {
            return stateResult;
        }

        Status = GoalStatus.Cancelled;
        UpdatedAt = utcNow;

        return Result.Success();
    }

    private static Result Validate(
        string title,
        string? description,
        int targetMinutes,
        int currentMinutes,
        DateOnly startDate,
        DateOnly deadline)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure(GoalErrors.TitleRequired);
        }

        if (title.Trim().Length > 100)
        {
            return Result.Failure(GoalErrors.TitleTooLong);
        }

        if (description?.Trim().Length > 1000)
        {
            return Result.Failure(GoalErrors.DescriptionTooLong);
        }

        if (targetMinutes <= 0)
        {
            return Result.Failure(GoalErrors.TargetMinutesInvalid);
        }

        if (currentMinutes < 0)
        {
            return Result.Failure(GoalErrors.CurrentMinutesInvalid);
        }

        if (deadline < startDate)
        {
            return Result.Failure(GoalErrors.DeadlineBeforeStartDate);
        }

        return Result.Success();
    }

    private Result EnsureCanBeModified()
    {
        if (Status == GoalStatus.Completed)
        {
            return Result.Failure(GoalErrors.CompletedGoalCannotBeModified);
        }

        if (Status == GoalStatus.Cancelled)
        {
            return Result.Failure(GoalErrors.CancelledGoalCannotBeModified);
        }

        return Result.Success();
    }

    private void RecalculateStatus()
    {
        if (Status is GoalStatus.Completed or GoalStatus.Cancelled)
        {
            return;
        }

        Status = CurrentMinutes >= TargetMinutes
            ? GoalStatus.ReadyToComplete
            : GoalStatus.Active;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
