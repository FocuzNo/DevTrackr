using DevTrackr.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using StatisticsService.Infrastructure.Caching;

namespace StatisticsService.Infrastructure.Messaging;

public sealed class StudySessionLoggedConsumer(
    IStatisticsDashboardCache cache,
    ILogger<StudySessionLoggedConsumer> logger) : IConsumer<StudySessionLoggedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<StudySessionLoggedIntegrationEvent> context)
    {
        await cache.InvalidateAsync(context.Message.UserId, context.CancellationToken);
        logger.LogInformation("Statistics cache invalidated after study session for UserId {UserId}", context.Message.UserId);
    }
}
