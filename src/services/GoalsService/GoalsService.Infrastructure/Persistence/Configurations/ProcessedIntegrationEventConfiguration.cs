using GoalsService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalsService.Infrastructure.Persistence.Configurations;

public sealed class ProcessedIntegrationEventConfiguration : IEntityTypeConfiguration<ProcessedIntegrationEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedIntegrationEvent> builder)
    {
        builder.ToTable("processed_integration_events");
        builder.HasKey(x => x.EventId);
        builder.Property(x => x.EventType).HasMaxLength(300).IsRequired();
        builder.Property(x => x.ProcessedAtUtc).IsRequired();
    }
}
