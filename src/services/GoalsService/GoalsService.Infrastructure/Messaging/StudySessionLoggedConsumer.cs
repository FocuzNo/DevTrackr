using DevTrackr.Contracts;
using MassTransit;

namespace GoalsService.Infrastructure.Messaging;

public sealed class StudySessionLoggedConsumer(StudySessionLoggedIntegrationEventProcessor processor)
    : IConsumer<StudySessionLoggedIntegrationEvent>
{
    public Task Consume(ConsumeContext<StudySessionLoggedIntegrationEvent> context) =>
        processor.ProcessAsync(context.Message, context.CancellationToken);
}
