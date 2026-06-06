using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ActivityService.Infrastructure.Persistence;

public sealed class ActivityDbContextFactory : IDesignTimeDbContextFactory<ActivityDbContext>
{
    public ActivityDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Host=localhost;Port=5432;Database=devtrackr_activity;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<ActivityDbContext>();
        optionsBuilder.UseNpgsql(connectionString, x => x.MigrationsAssembly(typeof(ActivityDbContext).Assembly.FullName));

        return new ActivityDbContext(optionsBuilder.Options);
    }
}
