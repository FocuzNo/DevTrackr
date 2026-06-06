namespace DevTrackr.SharedKernel.Primitives;

public abstract class AggregateRoot<TId>(TId id) : Entity<TId>(id)
    where TId : notnull;
