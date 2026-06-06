using DevTrackr.Contracts;
using MassTransit;

namespace StatisticsService.Infrastructure.Messaging;

public sealed class GoalCreatedConsumer(
    GoalCreatedEventProcessor processor) : IConsumer<GoalCreatedIntegrationEvent>
{
    public Task Consume(ConsumeContext<GoalCreatedIntegrationEvent> context) =>
        processor.ProcessAsync(context.Message, context.CancellationToken);
}
