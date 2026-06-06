using ActivityService.Domain.Sessions;
using Xunit;

namespace ActivityService.UnitTests;

public sealed class StudySessionTests
{
    [Fact]
    public void Create_ValidStudySession_ReturnsSuccess()
    {
        var result = StudySession.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "MassTransit outbox deep dive",
            90,
            StudySessionDifficulty.Hard,
            "Worked through publish flow.",
            new DateOnly(2026, 6, 6),
            new DateTime(2026, 6, 6, 12, 0, 0, DateTimeKind.Utc));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(90, result.Value!.DurationMinutes);
        Assert.Equal(StudySessionDifficulty.Hard, result.Value.Difficulty);
    }

    [Fact]
    public void Create_ZeroDuration_ReturnsFailure()
    {
        var result = Create(durationMinutes: 0);

        Assert.True(result.IsFailure);
        Assert.Equal(StudySessionErrors.DurationInvalid, result.Error);
    }

    [Fact]
    public void Create_DurationAboveLimit_ReturnsFailure()
    {
        var result = Create(durationMinutes: 481);

        Assert.True(result.IsFailure);
        Assert.Equal(StudySessionErrors.DurationTooLong, result.Error);
    }

    [Fact]
    public void Create_InvalidDifficulty_ReturnsFailure()
    {
        var result = StudySession.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "MassTransit outbox deep dive",
            90,
            (StudySessionDifficulty)6,
            null,
            new DateOnly(2026, 6, 6),
            new DateTime(2026, 6, 6, 12, 0, 0, DateTimeKind.Utc));

        Assert.True(result.IsFailure);
        Assert.Equal(StudySessionErrors.DifficultyInvalid, result.Error);
    }

    [Fact]
    public void Create_EmptyTopic_ReturnsFailure()
    {
        var result = Create(topic: "   ");

        Assert.True(result.IsFailure);
        Assert.Equal(StudySessionErrors.TopicRequired, result.Error);
    }

    [Fact]
    public void Create_FutureSessionDate_ReturnsFailure()
    {
        var utcNow = new DateTime(2026, 6, 6, 12, 0, 0, DateTimeKind.Utc);
        var result = StudySession.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "MassTransit outbox deep dive",
            90,
            StudySessionDifficulty.Medium,
            null,
            new DateOnly(2026, 6, 7),
            utcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(StudySessionErrors.SessionDateInFuture, result.Error);
    }

    private static DevTrackr.SharedKernel.Primitives.Result<StudySession> Create(
        string topic = "MassTransit outbox deep dive",
        int durationMinutes = 90) =>
        StudySession.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            topic,
            durationMinutes,
            StudySessionDifficulty.Medium,
            "Worked through publish flow.",
            new DateOnly(2026, 6, 6),
            new DateTime(2026, 6, 6, 12, 0, 0, DateTimeKind.Utc));
}
