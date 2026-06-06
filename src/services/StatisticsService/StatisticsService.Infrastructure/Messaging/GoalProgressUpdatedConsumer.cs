using DevTrackr.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using StatisticsService.Infrastructure.Caching;

namespace StatisticsService.Infrastructure.Messaging;

public sealed class GoalProgressUpdatedConsumer(
    IStatisticsDashboardCache cache,
    ILogger<GoalProgressUpdatedConsumer> logger) : IConsumer<GoalProgressUpdatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<GoalProgressUpdatedIntegrationEvent> context)
    {
        await cache.InvalidateAsync(context.Message.UserId, context.CancellationToken);
        logger.LogInformation("Statistics cache invalidated after goal progress update for UserId {UserId}", context.Message.UserId);
    }
}
