using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StatisticsService.Application.Dashboard;
using System.Text.Json;

namespace StatisticsService.Infrastructure.Caching;

public sealed class StatisticsDashboardCache(
    IDistributedCache cache,
    IOptions<StatisticsCacheOptions> options) : IStatisticsDashboardCache
{
    public async Task<DashboardResponse?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var payload = await cache.GetStringAsync(GetKey(userId), cancellationToken);
        return payload is null ? null : JsonSerializer.Deserialize<DashboardResponse>(payload);
    }

    public Task SetAsync(DashboardResponse dashboard, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(dashboard);

        return cache.SetStringAsync(
            GetKey(dashboard.UserId),
            payload,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Value.DashboardTtlMinutes)
            },
            cancellationToken);
    }

    public Task InvalidateAsync(Guid userId, CancellationToken cancellationToken = default) =>
        cache.RemoveAsync(GetKey(userId), cancellationToken);

    private static string GetKey(Guid userId) => $"statistics:dashboard:{userId}";
}
