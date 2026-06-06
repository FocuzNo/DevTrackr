using DevTrackr.Contracts;
using MassTransit;

namespace StatisticsService.Infrastructure.Messaging;

public sealed class GoalProgressUpdatedConsumer(
    GoalProgressUpdatedEventProcessor processor) : IConsumer<GoalProgressUpdatedIntegrationEvent>
{
    public Task Consume(ConsumeContext<GoalProgressUpdatedIntegrationEvent> context) =>
        processor.ProcessAsync(context.Message, context.CancellationToken);
}
