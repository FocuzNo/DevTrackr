using StatisticsService.Domain.Statistics;
using Xunit;

namespace StatisticsService.UnitTests;

public sealed class UserStatisticsTests
{
    [Fact]
    public void ApplyGoalCreated_IncrementsActiveGoals()
    {
        var statistics = UserStatistics.Create(Guid.NewGuid(), DateTime.UtcNow);

        statistics.ApplyGoalCreated(DateTime.UtcNow);

        Assert.Equal(1, statistics.ActiveGoals);
    }

    [Fact]
    public void ApplyGoalCompleted_DecrementsActiveGoals_AndIncrementsCompletedGoals()
    {
        var statistics = UserStatistics.Create(Guid.NewGuid(), DateTime.UtcNow);
        statistics.ApplyGoalCreated(DateTime.UtcNow);
        statistics.ApplyGoalCreated(DateTime.UtcNow);

        statistics.ApplyGoalCompleted(DateTime.UtcNow);

        Assert.Equal(1, statistics.ActiveGoals);
        Assert.Equal(1, statistics.CompletedGoals);
    }

    [Fact]
    public void ApplyGoalCancelled_DecrementsActiveGoals()
    {
        var statistics = UserStatistics.Create(Guid.NewGuid(), DateTime.UtcNow);
        statistics.ApplyGoalCreated(DateTime.UtcNow);

        statistics.ApplyGoalCancelled(DateTime.UtcNow);

        Assert.Equal(0, statistics.ActiveGoals);
    }

    [Fact]
    public void ActiveGoals_NeverBecomesNegative()
    {
        var statistics = UserStatistics.Create(Guid.NewGuid(), DateTime.UtcNow);

        statistics.ApplyGoalCancelled(DateTime.UtcNow);
        statistics.ApplyGoalCompleted(DateTime.UtcNow);

        Assert.Equal(0, statistics.ActiveGoals);
    }

    [Fact]
    public void ApplyGoalProgress_DoesNotModifyActiveGoals()
    {
        var statistics = UserStatistics.Create(Guid.NewGuid(), DateTime.UtcNow);
        statistics.ApplyGoalCreated(DateTime.UtcNow);

        statistics.ApplyGoalProgress(DateTime.UtcNow);

        Assert.Equal(1, statistics.ActiveGoals);
    }
}
