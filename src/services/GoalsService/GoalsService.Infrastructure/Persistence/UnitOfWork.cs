using GoalsService.Application.Abstractions.Persistence;

namespace GoalsService.Infrastructure.Persistence;

public sealed class UnitOfWork(GoalsDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
