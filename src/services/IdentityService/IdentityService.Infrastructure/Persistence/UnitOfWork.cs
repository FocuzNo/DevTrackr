using IdentityService.Application.Abstractions.Persistence;

namespace IdentityService.Infrastructure.Persistence;

public sealed class UnitOfWork(IdentityDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
