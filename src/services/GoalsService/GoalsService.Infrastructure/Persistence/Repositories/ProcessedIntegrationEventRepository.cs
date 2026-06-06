using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoalsService.Infrastructure.Persistence.Repositories;

public sealed class ProcessedIntegrationEventRepository(GoalsDbContext dbContext) : IProcessedIntegrationEventRepository
{
    public Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken = default) =>
        dbContext.ProcessedIntegrationEvents.AnyAsync(x => x.EventId == eventId, cancellationToken);

    public Task AddAsync(Guid eventId, string eventType, DateTime processedAtUtc, CancellationToken cancellationToken = default) =>
        dbContext.ProcessedIntegrationEvents.AddAsync(
            new ProcessedIntegrationEvent
            {
                EventId = eventId,
                EventType = eventType,
                ProcessedAtUtc = processedAtUtc
            },
            cancellationToken).AsTask();
}
