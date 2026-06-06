using GoalsService.Domain.Goals;
using Xunit;

namespace GoalsService.UnitTests;

public sealed class GoalTests
{
    [Fact]
    public void Create_ValidGoal_ReturnsSuccess()
    {
        var result = Goal.Create(
            Guid.NewGuid(),
            "Master MassTransit",
            "Learn outbox and consumers",
            GoalCategory.Architecture,
            600,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30),
            DateTime.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(GoalStatus.Active, result.Value!.Status);
        Assert.Equal(0, result.Value.CurrentMinutes);
    }

    [Fact]
    public void Create_InvalidTargetMinutes_ReturnsFailure()
    {
        var result = Goal.Create(
            Guid.NewGuid(),
            "Master MassTransit",
            null,
            GoalCategory.Architecture,
            0,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30),
            DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(GoalErrors.TargetMinutesInvalid, result.Error);
    }

    [Fact]
    public void Complete_BelowEightyPercent_ReturnsFailure()
    {
        var goal = CreateGoal(600);
        goal.AddProgress(300, DateTime.UtcNow);

        var result = goal.Complete(DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(GoalErrors.GoalCannotBeCompletedYet, result.Error);
    }

    [Fact]
    public void Complete_AtLeastEightyPercent_ReturnsSuccess()
    {
        var goal = CreateGoal(600);
        goal.AddProgress(480, DateTime.UtcNow);

        var result = goal.Complete(DateTime.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.Equal(GoalStatus.Completed, goal.Status);
        Assert.NotNull(goal.CompletedAt);
    }

    [Fact]
    public void Update_CompletedGoal_ReturnsFailure()
    {
        var goal = CreateGoal(600);
        goal.AddProgress(600, DateTime.UtcNow);
        goal.Complete(DateTime.UtcNow);

        var result = goal.UpdateDetails(
            "Updated",
            "Updated description",
            GoalCategory.DotNet,
            700,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 7, 1),
            DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(GoalErrors.CompletedGoalCannotBeModified, result.Error);
    }

    [Fact]
    public void AddProgress_CancelledGoal_ReturnsFailure()
    {
        var goal = CreateGoal(600);
        goal.Cancel(DateTime.UtcNow);

        var result = goal.AddProgress(30, DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(GoalErrors.CancelledGoalCannotReceiveProgress, result.Error);
    }

    [Fact]
    public void AddProgress_ReachingTarget_SetsReadyToComplete()
    {
        var goal = CreateGoal(600);

        var result = goal.AddProgress(600, DateTime.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.Equal(GoalStatus.ReadyToComplete, goal.Status);
        Assert.Equal(100m, goal.ProgressPercentage);
    }

    private static Goal CreateGoal(int targetMinutes) =>
        Goal.Create(
            Guid.NewGuid(),
            "Master MassTransit",
            "Learn outbox and consumers",
            GoalCategory.Architecture,
            targetMinutes,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30),
            DateTime.UtcNow).Value!;
}
