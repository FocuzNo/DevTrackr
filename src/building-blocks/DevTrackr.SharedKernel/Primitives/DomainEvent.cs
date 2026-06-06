namespace DevTrackr.SharedKernel.Primitives;

public abstract record DomainEvent(Guid EventId, DateTime OccurredOnUtc) : IDomainEvent;
