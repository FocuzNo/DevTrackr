using MassTransit;
using Microsoft.Extensions.Logging;

namespace DevTrackr.Messaging;

public sealed class LoggingPublishObserver(ILogger<LoggingPublishObserver> logger) : IPublishObserver
{
    public Task PrePublish<T>(PublishContext<T> context)
        where T : class
    {
        logger.LogInformation("Publishing integration event {MessageType} with CorrelationId {CorrelationId}",
            typeof(T).Name,
            context.CorrelationId);

        return Task.CompletedTask;
    }

    public Task PostPublish<T>(PublishContext<T> context)
        where T : class => Task.CompletedTask;

    public Task PublishFault<T>(PublishContext<T> context, Exception exception)
        where T : class
    {
        logger.LogError(exception, "Failed to publish integration event {MessageType}", typeof(T).Name);
        return Task.CompletedTask;
    }
}
