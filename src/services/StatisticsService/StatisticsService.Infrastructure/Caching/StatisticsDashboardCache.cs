using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StatisticsService.Application.Abstractions.Caching;
using StatisticsService.Application.Statistics.Responses;
using System.Text.Json;

namespace StatisticsService.Infrastructure.Caching;

public sealed class StatisticsDashboardCache(
    IDistributedCache cache,
    IOptions<StatisticsCacheOptions> options) : IDashboardCache
{
    public async Task<DashboardResponse?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var payload = await cache.GetStringAsync(GetKey(userId), cancellationToken);
        return payload is null ? null : JsonSerializer.Deserialize<DashboardResponse>(payload);
    }

    public Task SetAsync(Guid userId, DashboardResponse dashboard, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(dashboard);

        return cache.SetStringAsync(
            GetKey(userId),
            payload,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Value.DashboardTtlMinutes)
            },
            cancellationToken);
    }

    public Task InvalidateAsync(Guid userId, CancellationToken cancellationToken = default) =>
        cache.RemoveAsync(GetKey(userId), cancellationToken);

    private static string GetKey(Guid userId) => $"devtrackr:statistics:dashboard:{userId}";
}
