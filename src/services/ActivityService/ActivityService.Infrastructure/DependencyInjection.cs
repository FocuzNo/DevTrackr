using ActivityService.Infrastructure.Persistence;
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
        services.AddHealthChecks();

        services.AddDevTrackrMassTransit<ActivityDbContext>(
            configuration,
            useEntityFrameworkOutbox: true);

        return services;
    }
}
