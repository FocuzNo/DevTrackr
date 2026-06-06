using DevTrackr.Contracts;
using MassTransit;

namespace StatisticsService.Infrastructure.Messaging;

public sealed class GoalCompletedConsumer(
    GoalCompletedEventProcessor processor) : IConsumer<GoalCompletedIntegrationEvent>
{
    public Task Consume(ConsumeContext<GoalCompletedIntegrationEvent> context) =>
        processor.ProcessAsync(context.Message, context.CancellationToken);
}
