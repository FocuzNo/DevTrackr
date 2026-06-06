using DevTrackr.SharedKernel.Primitives;

namespace ActivityService.Domain.Sessions;

public sealed class StudySession : AggregateRoot<Guid>
{
    private StudySession(
        Guid id,
        Guid userId,
        Guid goalId,
        string topic,
        int durationMinutes,
        StudySessionDifficulty difficulty,
        string? note,
        DateOnly sessionDate,
        DateTime createdAt,
        DateTime updatedAt)
        : base(id)
    {
        UserId = userId;
        GoalId = goalId;
        Topic = topic;
        DurationMinutes = durationMinutes;
        Difficulty = difficulty;
        Note = note;
        SessionDate = sessionDate;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    private StudySession()
        : base(Guid.Empty)
    {
    }

    public Guid UserId { get; private set; }

    public Guid GoalId { get; private set; }

    public string Topic { get; private set; } = string.Empty;

    public int DurationMinutes { get; private set; }

    public StudySessionDifficulty Difficulty { get; private set; }

    public string? Note { get; private set; }

    public DateOnly SessionDate { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public static Result<StudySession> Create(
        Guid userId,
        Guid goalId,
        string topic,
        int durationMinutes,
        StudySessionDifficulty difficulty,
        string? note,
        DateOnly sessionDate,
        DateTime utcNow)
    {
        var validationResult = Validate(userId, goalId, topic, durationMinutes, difficulty, note, sessionDate, utcNow);
        if (validationResult.IsFailure)
        {
            return Result<StudySession>.Failure(validationResult.Error);
        }

        var normalizedTopic = topic.Trim();
        var normalizedNote = Normalize(note);

        var studySession = new StudySession(
            Guid.NewGuid(),
            userId,
            goalId,
            normalizedTopic,
            durationMinutes,
            difficulty,
            normalizedNote,
            sessionDate,
            utcNow,
            utcNow);

        return Result<StudySession>.Success(studySession);
    }

    public Result UpdateDetails(
        string topic,
        int durationMinutes,
        StudySessionDifficulty difficulty,
        string? note,
        DateOnly sessionDate,
        DateTime utcNow)
    {
        var validationResult = Validate(UserId, GoalId, topic, durationMinutes, difficulty, note, sessionDate, utcNow);
        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        Topic = topic.Trim();
        DurationMinutes = durationMinutes;
        Difficulty = difficulty;
        Note = Normalize(note);
        SessionDate = sessionDate;
        UpdatedAt = utcNow;

        return Result.Success();
    }

    private static Result Validate(
        Guid userId,
        Guid goalId,
        string topic,
        int durationMinutes,
        StudySessionDifficulty difficulty,
        string? note,
        DateOnly sessionDate,
        DateTime utcNow)
    {
        if (userId == Guid.Empty)
        {
            return Result.Failure(StudySessionErrors.UserIdRequired);
        }

        if (goalId == Guid.Empty)
        {
            return Result.Failure(StudySessionErrors.GoalIdRequired);
        }

        if (string.IsNullOrWhiteSpace(topic))
        {
            return Result.Failure(StudySessionErrors.TopicRequired);
        }

        if (topic.Trim().Length > 100)
        {
            return Result.Failure(StudySessionErrors.TopicTooLong);
        }

        if (note?.Trim().Length > 1000)
        {
            return Result.Failure(StudySessionErrors.NoteTooLong);
        }

        if (durationMinutes <= 0)
        {
            return Result.Failure(StudySessionErrors.DurationInvalid);
        }

        if (durationMinutes > 480)
        {
            return Result.Failure(StudySessionErrors.DurationTooLong);
        }

        if ((int)difficulty is < 1 or > 5)
        {
            return Result.Failure(StudySessionErrors.DifficultyInvalid);
        }

        if (sessionDate > DateOnly.FromDateTime(utcNow))
        {
            return Result.Failure(StudySessionErrors.SessionDateInFuture);
        }

        return Result.Success();
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
