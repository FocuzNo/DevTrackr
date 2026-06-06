using DevTrackr.SharedKernel.Primitives;

namespace StatisticsService.Domain.Statistics;

public sealed class UserStatistics : Entity
{
    private UserStatistics()
        : base(Guid.Empty)
    {
    }

    public Guid UserId { get; private set; }

    public int TotalStudyMinutes { get; private set; }

    public int TotalSessions { get; private set; }

    public int CompletedGoals { get; private set; }

    public int ActiveGoals { get; private set; }

    public int CurrentStreak { get; private set; }

    public int LongestStreak { get; private set; }

    public decimal AverageDifficulty { get; private set; }

    public DateOnly? LastStudyDate { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public static UserStatistics Create(
        Guid userId,
        DateTime updatedAtUtc) =>
        new(Guid.NewGuid())
        {
            UserId = userId,
            UpdatedAt = updatedAtUtc
        };

    private UserStatistics(Guid id)
        : base(id)
    {
    }

    public void ApplyStudySession(
        int durationMinutes,
        int difficulty,
        DateOnly sessionDate,
        int currentStreak,
        int longestStreak,
        DateTime updatedAtUtc)
    {
        AverageDifficulty = CalculateAverage(AverageDifficulty, TotalSessions, difficulty);
        TotalStudyMinutes += durationMinutes;
        TotalSessions++;
        LastStudyDate = LastStudyDate is null || sessionDate > LastStudyDate ? sessionDate : LastStudyDate;
        CurrentStreak = currentStreak;
        LongestStreak = Math.Max(LongestStreak, longestStreak);
        UpdatedAt = updatedAtUtc;
    }

    public void ApplyGoalProgress(DateTime updatedAtUtc)
    {
        UpdatedAt = updatedAtUtc;
    }

    public void ApplyGoalCreated(DateTime updatedAtUtc)
    {
        ActiveGoals++;
        UpdatedAt = updatedAtUtc;
    }

    public void ApplyGoalCancelled(DateTime updatedAtUtc)
    {
        ActiveGoals = Math.Max(0, ActiveGoals - 1);
        UpdatedAt = updatedAtUtc;
    }

    public void ApplyGoalCompleted(DateTime updatedAtUtc)
    {
        CompletedGoals++;
        ActiveGoals = Math.Max(0, ActiveGoals - 1);
        UpdatedAt = updatedAtUtc;
    }

    private static decimal CalculateAverage(decimal currentAverage, int currentCount, int nextValue)
    {
        if (currentCount == 0)
        {
            return nextValue;
        }

        return decimal.Round(((currentAverage * currentCount) + nextValue) / (currentCount + 1), 2);
    }
}
