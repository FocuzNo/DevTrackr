using DevTrackr.Contracts;
using MassTransit;

namespace StatisticsService.Infrastructure.Messaging;

public sealed class StudySessionLoggedConsumer(
    StudySessionLoggedEventProcessor processor) : IConsumer<StudySessionLoggedIntegrationEvent>
{
    public Task Consume(ConsumeContext<StudySessionLoggedIntegrationEvent> context) =>
        processor.ProcessAsync(context.Message, context.CancellationToken);
}
