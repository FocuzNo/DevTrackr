using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StatisticsService.Infrastructure.Persistence.Entities;

namespace StatisticsService.Infrastructure.Persistence.Configurations;

public sealed class ProcessedIntegrationEventConfiguration : IEntityTypeConfiguration<ProcessedIntegrationEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedIntegrationEvent> builder)
    {
        builder.ToTable("processed_integration_events");

        builder.HasKey(x => x.EventId);

        builder.Property(x => x.EventId).HasColumnName("event_id");
        builder.Property(x => x.EventType).HasColumnName("event_type").HasMaxLength(200);
        builder.Property(x => x.ProcessedAtUtc).HasColumnName("processed_at_utc");
    }
}
