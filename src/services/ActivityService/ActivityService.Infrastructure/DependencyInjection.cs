using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Infrastructure.HealthChecks;
using ActivityService.Infrastructure.Persistence;
using ActivityService.Infrastructure.Persistence.Repositories;
using DevTrackr.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ActivityService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("ActivityService database connection string is missing.");

        services.AddDbContext<ActivityDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                x => x.MigrationsAssembly(typeof(ActivityDbContext).Assembly.FullName)));

        services.AddScoped<IStudySessionRepository, StudySessionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHealthChecks()
            .AddCheck<ActivityDbContextHealthCheck>("postgresql")
            .AddCheck<RabbitMqOptionsHealthCheck>("rabbitmq");

        services.AddDevTrackrMassTransit<ActivityDbContext>(
            configuration,
            useEntityFrameworkOutbox: true,
            endpointNamePrefix: "activity");

        return services;
    }
}
