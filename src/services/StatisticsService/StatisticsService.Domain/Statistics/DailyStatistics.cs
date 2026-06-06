using DevTrackr.SharedKernel.Primitives;

namespace StatisticsService.Domain.Statistics;

public sealed class DailyStatistics : Entity
{
    private DailyStatistics()
        : base(Guid.Empty)
    {
    }

    public Guid UserId { get; private set; }

    public DateOnly Date { get; private set; }

    public int TotalMinutes { get; private set; }

    public int SessionsCount { get; private set; }

    public decimal AverageDifficulty { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public static DailyStatistics Create(
        Guid userId,
        DateOnly date,
        DateTime updatedAtUtc) =>
        new(Guid.NewGuid())
        {
            UserId = userId,
            Date = date,
            UpdatedAt = updatedAtUtc
        };

    private DailyStatistics(Guid id)
        : base(id)
    {
    }

    public void ApplyStudySession(
        int durationMinutes,
        int difficulty,
        DateTime updatedAtUtc)
    {
        AverageDifficulty = SessionsCount == 0
            ? difficulty
            : decimal.Round(((AverageDifficulty * SessionsCount) + difficulty) / (SessionsCount + 1), 2);

        TotalMinutes += durationMinutes;
        SessionsCount++;
        UpdatedAt = updatedAtUtc;
    }
}
