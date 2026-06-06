namespace StatisticsService.Infrastructure.Caching;

public sealed class StatisticsCacheOptions
{
    public const string SectionName = "Cache";

    public int DashboardTtlMinutes { get; init; } = 10;
}
