using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StatisticsService.Infrastructure.Persistence;

public sealed class StatisticsDbContextFactory : IDesignTimeDbContextFactory<StatisticsDbContext>
{
    public StatisticsDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Host=localhost;Port=5432;Database=devtrackr_statistics;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<StatisticsDbContext>();
        optionsBuilder.UseNpgsql(connectionString, x => x.MigrationsAssembly(typeof(StatisticsDbContext).Assembly.FullName));

        return new StatisticsDbContext(optionsBuilder.Options);
    }
}
