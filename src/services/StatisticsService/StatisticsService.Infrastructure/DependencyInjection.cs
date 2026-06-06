using DevTrackr.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StatisticsService.Infrastructure.Caching;
using StatisticsService.Infrastructure.Messaging;
using StatisticsService.Infrastructure.Persistence;

namespace StatisticsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("StatisticsService database connection string is missing.");

        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("StatisticsService Redis connection string is missing.");

        services.AddDbContext<StatisticsDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                x => x.MigrationsAssembly(typeof(StatisticsDbContext).Assembly.FullName)));
        services.AddHealthChecks();

        services.Configure<StatisticsCacheOptions>(configuration.GetSection(StatisticsCacheOptions.SectionName));
        services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);
        services.AddScoped<IStatisticsDashboardCache, StatisticsDashboardCache>();

        services.AddDevTrackrMassTransit<StatisticsDbContext>(
            configuration,
            configureConsumers: x =>
            {
                x.AddConsumer<StudySessionLoggedConsumer>();
                x.AddConsumer<GoalProgressUpdatedConsumer>();
                x.AddConsumer<GoalCompletedConsumer>();
            });

        return services;
    }
}
