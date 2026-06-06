using StatisticsService.Application.Statistics.Responses;

namespace StatisticsService.Application.Abstractions.Caching;

public interface IDashboardCache
{
    Task<DashboardResponse?> GetAsync(Guid userId, CancellationToken cancellationToken = default);

    Task SetAsync(Guid userId, DashboardResponse dashboard, CancellationToken cancellationToken = default);

    Task InvalidateAsync(Guid userId, CancellationToken cancellationToken = default);
}
