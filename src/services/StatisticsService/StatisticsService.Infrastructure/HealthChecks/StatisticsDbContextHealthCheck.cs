using Microsoft.Extensions.Diagnostics.HealthChecks;
using StatisticsService.Infrastructure.Persistence;

namespace StatisticsService.Infrastructure.HealthChecks;

public sealed class StatisticsDbContextHealthCheck(StatisticsDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return await dbContext.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy("PostgreSQL is reachable.")
            : HealthCheckResult.Unhealthy("PostgreSQL is not reachable.");
    }
}
