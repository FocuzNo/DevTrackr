using StatisticsService.Application.Dashboard;

namespace StatisticsService.Infrastructure.Caching;

public interface IStatisticsDashboardCache
{
    Task<DashboardResponse?> GetAsync(Guid userId, CancellationToken cancellationToken = default);

    Task SetAsync(DashboardResponse dashboard, CancellationToken cancellationToken = default);

    Task InvalidateAsync(Guid userId, CancellationToken cancellationToken = default);
}
