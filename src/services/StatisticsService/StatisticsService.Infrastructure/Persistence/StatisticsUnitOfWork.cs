namespace StatisticsService.Infrastructure.Persistence;

public sealed class StatisticsUnitOfWork(StatisticsDbContext dbContext) : IStatisticsUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
