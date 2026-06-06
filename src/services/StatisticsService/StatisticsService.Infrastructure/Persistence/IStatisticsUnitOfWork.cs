namespace StatisticsService.Infrastructure.Persistence;

public interface IStatisticsUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
