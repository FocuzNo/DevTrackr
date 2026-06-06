using Microsoft.EntityFrameworkCore;
using StatisticsService.Infrastructure.Persistence.Entities;

namespace StatisticsService.Infrastructure.Persistence.Repositories;

public sealed class ProcessedIntegrationEventRepository(StatisticsDbContext dbContext) : IProcessedIntegrationEventRepository
{
    public Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken = default) =>
        dbContext.ProcessedIntegrationEvents.AnyAsync(x => x.EventId == eventId, cancellationToken);

    public Task AddAsync(ProcessedIntegrationEvent processedEvent, CancellationToken cancellationToken = default) =>
        dbContext.ProcessedIntegrationEvents.AddAsync(processedEvent, cancellationToken).AsTask();
}
