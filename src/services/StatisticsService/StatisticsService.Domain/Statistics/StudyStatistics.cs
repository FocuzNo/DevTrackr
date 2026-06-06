using DevTrackr.SharedKernel.Primitives;

namespace StatisticsService.Domain.Statistics;

public sealed class StudyStatistics : AggregateRoot<Guid>
{
    private StudyStatistics(Guid id, Guid userId)
        : base(id)
    {
        UserId = userId;
    }

    public Guid UserId { get; private set; }

    public int TotalSessions { get; private set; }

    public int TotalMinutes { get; private set; }

    public int CompletedGoals { get; private set; }

    public static StudyStatistics Create(Guid userId) => new(Guid.NewGuid(), userId);

    public void AddSession(int durationMinutes)
    {
        TotalSessions++;
        TotalMinutes += durationMinutes;
    }

    public void MarkGoalCompleted()
    {
        CompletedGoals++;
    }
}
