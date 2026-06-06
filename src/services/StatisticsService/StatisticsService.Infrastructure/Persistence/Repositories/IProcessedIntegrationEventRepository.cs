using StatisticsService.Infrastructure.Persistence.Entities;

namespace StatisticsService.Infrastructure.Persistence.Repositories;

public interface IProcessedIntegrationEventRepository
{
    Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task AddAsync(ProcessedIntegrationEvent processedEvent, CancellationToken cancellationToken = default);
}
