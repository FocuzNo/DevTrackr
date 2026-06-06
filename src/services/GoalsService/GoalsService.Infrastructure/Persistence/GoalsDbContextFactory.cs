using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GoalsService.Infrastructure.Persistence;

public sealed class GoalsDbContextFactory : IDesignTimeDbContextFactory<GoalsDbContext>
{
    public GoalsDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Host=localhost;Port=5432;Database=devtrackr_goals;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<GoalsDbContext>();
        optionsBuilder.UseNpgsql(connectionString, x => x.MigrationsAssembly(typeof(GoalsDbContext).Assembly.FullName));

        return new GoalsDbContext(optionsBuilder.Options);
    }
}
