using DevTrackr.Messaging;
using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Infrastructure.HealthChecks;
using GoalsService.Infrastructure.Messaging;
using GoalsService.Infrastructure.Persistence;
using GoalsService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoalsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("GoalsService database connection string is missing.");

        services.AddDbContext<GoalsDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                x => x.MigrationsAssembly(typeof(GoalsDbContext).Assembly.FullName)));

        services.AddScoped<IGoalRepository, GoalRepository>();
        services.AddScoped<IProcessedIntegrationEventRepository, ProcessedIntegrationEventRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<StudySessionLoggedIntegrationEventProcessor>();

        services.AddHealthChecks()
            .AddCheck<GoalsDbContextHealthCheck>("postgresql")
            .AddCheck<RabbitMqOptionsHealthCheck>("rabbitmq");

        services.AddDevTrackrMassTransit<GoalsDbContext>(
            configuration,
            configureConsumers: x => x.AddConsumer<StudySessionLoggedConsumer>(),
            useEntityFrameworkOutbox: true);

        return services;
    }
}
