namespace DevTrackr.SharedKernel.Primitives;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOnUtc { get; }
}
