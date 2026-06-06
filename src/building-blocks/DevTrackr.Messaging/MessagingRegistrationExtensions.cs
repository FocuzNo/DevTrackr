using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DevTrackr.Messaging;

public static class MessagingRegistrationExtensions
{
    public static IServiceCollection AddDevTrackrMassTransit<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureConsumers = null,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? configureBus = null,
        bool useEntityFrameworkOutbox = false)
        where TDbContext : DbContext
    {
        var rabbitMqOptions = configuration
            .GetSection(RabbitMqOptions.SectionName)
            .Get<RabbitMqOptions>();

        if (rabbitMqOptions is null)
        {
            throw new InvalidOperationException("RabbitMQ configuration is missing.");
        }

        services.Configure<RabbitMqOptions>(
            configuration.GetSection(RabbitMqOptions.SectionName));
        services.AddSingleton<LoggingPublishObserver>();
        services.AddSingleton<LoggingConsumeObserver>();

        services.AddMassTransit(x =>
        {
            configureConsumers?.Invoke(x);

            if (useEntityFrameworkOutbox)
            {
                x.AddEntityFrameworkOutbox<TDbContext>(outbox =>
                {
                    outbox.QueryDelay = TimeSpan.FromSeconds(5);
                    outbox.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                    outbox.UsePostgres();
                    outbox.UseBusOutbox();
                });
            }

            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                cfg.Host(options.Host, options.VirtualHost, host =>
                {
                    host.Username(options.Username);
                    host.Password(options.Password);
                });

                cfg.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(5)));
                cfg.ConfigureEndpoints(context);
                cfg.ConnectPublishObserver(context.GetRequiredService<LoggingPublishObserver>());
                cfg.ConnectConsumeObserver(context.GetRequiredService<LoggingConsumeObserver>());

                configureBus?.Invoke(context, cfg);
            });
        });

        return services;
    }
}
