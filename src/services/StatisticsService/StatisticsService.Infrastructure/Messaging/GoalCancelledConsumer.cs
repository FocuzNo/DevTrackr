using DevTrackr.Contracts;
using MassTransit;

namespace StatisticsService.Infrastructure.Messaging;

public sealed class GoalCancelledConsumer(
    GoalCancelledEventProcessor processor) : IConsumer<GoalCancelledIntegrationEvent>
{
    public Task Consume(ConsumeContext<GoalCancelledIntegrationEvent> context) =>
        processor.ProcessAsync(context.Message, context.CancellationToken);
}
