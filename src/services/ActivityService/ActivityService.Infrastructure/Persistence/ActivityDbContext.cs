using ActivityService.Domain.Sessions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ActivityService.Infrastructure.Persistence;

public sealed class ActivityDbContext(DbContextOptions<ActivityDbContext> options) : DbContext(options)
{
    public DbSet<StudySession> StudySessions => Set<StudySession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudySession>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Topic).HasMaxLength(150);
        });

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
