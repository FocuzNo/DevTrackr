using DevTrackr.SharedKernel.Primitives;

namespace ActivityService.Domain.Sessions;

public sealed class StudySession : AggregateRoot<Guid>
{
    private StudySession(Guid id, Guid userId, Guid goalId, string topic, int durationMinutes, DateOnly sessionDate)
        : base(id)
    {
        UserId = userId;
        GoalId = goalId;
        Topic = topic;
        DurationMinutes = durationMinutes;
        SessionDate = sessionDate;
    }

    public Guid UserId { get; private set; }

    public Guid GoalId { get; private set; }

    public string Topic { get; private set; }

    public int DurationMinutes { get; private set; }

    public DateOnly SessionDate { get; private set; }

    public static StudySession Create(Guid userId, Guid goalId, string topic, int durationMinutes, DateOnly sessionDate) =>
        new(Guid.NewGuid(), userId, goalId, topic.Trim(), durationMinutes, sessionDate);
}
