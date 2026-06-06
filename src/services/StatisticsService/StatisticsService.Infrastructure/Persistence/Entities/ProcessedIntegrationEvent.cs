namespace StatisticsService.Infrastructure.Persistence.Entities;

public sealed class ProcessedIntegrationEvent
{
    public Guid EventId { get; set; }

    public string EventType { get; set; } = string.Empty;

    public DateTime ProcessedAtUtc { get; set; }
}
