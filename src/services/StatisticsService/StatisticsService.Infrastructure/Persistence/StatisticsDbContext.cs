using Microsoft.EntityFrameworkCore;
using StatisticsService.Domain.Statistics;

namespace StatisticsService.Infrastructure.Persistence;

public sealed class StatisticsDbContext(DbContextOptions<StatisticsDbContext> options) : DbContext(options)
{
    public DbSet<StudyStatistics> Statistics => Set<StudyStatistics>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudyStatistics>(builder =>
        {
            builder.HasKey(x => x.Id);
        });
    }
}
