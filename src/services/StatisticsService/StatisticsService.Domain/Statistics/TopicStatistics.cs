using DevTrackr.SharedKernel.Primitives;

namespace StatisticsService.Domain.Statistics;

public sealed class TopicStatistics : Entity
{
    private TopicStatistics()
        : base(Guid.Empty)
    {
    }

    public Guid UserId { get; private set; }

    public string Topic { get; private set; } = string.Empty;

    public int TotalMinutes { get; private set; }

    public int SessionsCount { get; private set; }

    public decimal AverageDifficulty { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public static TopicStatistics Create(
        Guid userId,
        string topic,
        DateTime updatedAtUtc) =>
        new(Guid.NewGuid())
        {
            UserId = userId,
            Topic = topic,
            UpdatedAt = updatedAtUtc
        };

    private TopicStatistics(Guid id)
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
