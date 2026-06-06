using Microsoft.EntityFrameworkCore;
using StatisticsService.Domain.Statistics;
using StatisticsService.Infrastructure.Persistence.Entities;

namespace StatisticsService.Infrastructure.Persistence;

public sealed class StatisticsDbContext(DbContextOptions<StatisticsDbContext> options) : DbContext(options)
{
    public DbSet<UserStatistics> UserStatistics => Set<UserStatistics>();

    public DbSet<TopicStatistics> TopicStatistics => Set<TopicStatistics>();

    public DbSet<DailyStatistics> DailyStatistics => Set<DailyStatistics>();

    public DbSet<ProcessedIntegrationEvent> ProcessedIntegrationEvents => Set<ProcessedIntegrationEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StatisticsDbContext).Assembly);
    }
}
