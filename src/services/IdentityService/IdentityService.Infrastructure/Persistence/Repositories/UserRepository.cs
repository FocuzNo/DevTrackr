using IdentityService.Application.Abstractions.Persistence;
using IdentityService.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(IdentityDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddAsync(
        User entity,
        CancellationToken cancellationToken = default) =>
        dbContext.Users.AddAsync(entity, cancellationToken).AsTask();

    public void Update(User entity) => dbContext.Users.Update(entity);

    public void Remove(User entity) => dbContext.Users.Remove(entity);

    public Task<bool> ExistsByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default) =>
        dbContext.Users.AnyAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);

    public Task<User?> GetByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default) =>
        dbContext.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
}
