using DevTrackr.Messaging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace StatisticsService.Infrastructure.HealthChecks;

public sealed class RabbitMqOptionsHealthCheck(IOptions<RabbitMqOptions> options) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var value = options.Value;
        var healthy = !string.IsNullOrWhiteSpace(value.Host)
            && !string.IsNullOrWhiteSpace(value.Username)
            && !string.IsNullOrWhiteSpace(value.Password);

        return Task.FromResult(
            healthy
                ? HealthCheckResult.Healthy("RabbitMQ configuration is present.")
                : HealthCheckResult.Unhealthy("RabbitMQ configuration is missing."));
    }
}
