using DevTrackr.SharedKernel.Primitives;

namespace DevTrackr.SharedKernel.Persistence;

public interface IRepository<TEntity>
    where TEntity : Entity<Guid>
{
    Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    void Remove(TEntity entity);
}
