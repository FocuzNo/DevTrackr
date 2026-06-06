using MassTransit;
using Microsoft.Extensions.Logging;

namespace DevTrackr.Messaging;

public sealed class LoggingConsumeObserver(ILogger<LoggingConsumeObserver> logger) : IConsumeObserver
{
    public Task PreConsume<T>(ConsumeContext<T> context)
        where T : class
    {
        logger.LogInformation("Consuming integration event {MessageType} with MessageId {MessageId}",
            typeof(T).Name,
            context.MessageId);

        return Task.CompletedTask;
    }

    public Task PostConsume<T>(ConsumeContext<T> context)
        where T : class => Task.CompletedTask;

    public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
        where T : class
    {
        logger.LogError(exception, "Failed to consume integration event {MessageType}", typeof(T).Name);
        return Task.CompletedTask;
    }
}
