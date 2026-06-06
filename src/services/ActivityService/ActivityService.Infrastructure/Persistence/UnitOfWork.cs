using ActivityService.Application.Abstractions.Persistence;

namespace ActivityService.Infrastructure.Persistence;

public sealed class UnitOfWork(ActivityDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
