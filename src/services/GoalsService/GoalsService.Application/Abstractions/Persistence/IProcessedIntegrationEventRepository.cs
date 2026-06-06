namespace GoalsService.Application.Abstractions.Persistence;

public interface IProcessedIntegrationEventRepository
{
    Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task AddAsync(Guid eventId, string eventType, DateTime processedAtUtc, CancellationToken cancellationToken = default);
}
