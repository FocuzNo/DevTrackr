using DevTrackr.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using StatisticsService.Infrastructure.Caching;

namespace StatisticsService.Infrastructure.Messaging;

public sealed class GoalCompletedConsumer(
    IStatisticsDashboardCache cache,
    ILogger<GoalCompletedConsumer> logger) : IConsumer<GoalCompletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<GoalCompletedIntegrationEvent> context)
    {
        await cache.InvalidateAsync(context.Message.UserId, context.CancellationToken);
        logger.LogInformation("Statistics cache invalidated after goal completion for UserId {UserId}", context.Message.UserId);
    }
}
