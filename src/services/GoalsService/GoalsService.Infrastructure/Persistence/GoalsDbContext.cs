using GoalsService.Domain.Goals;
using GoalsService.Infrastructure.Persistence.Configurations;
using GoalsService.Infrastructure.Persistence.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace GoalsService.Infrastructure.Persistence;

public sealed class GoalsDbContext(DbContextOptions<GoalsDbContext> options) : DbContext(options)
{
    public DbSet<Goal> Goals => Set<Goal>();

    public DbSet<ProcessedIntegrationEvent> ProcessedIntegrationEvents => Set<ProcessedIntegrationEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GoalsDbContext).Assembly);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
