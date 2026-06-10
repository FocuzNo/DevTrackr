using IdentityService.Application.Abstractions.Persistence;
using IdentityService.Application.Users.Responses;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence.Repositories;

public sealed class UserReadRepository(IdentityDbContext dbContext) : IUserReadRepository
{
    public Task<CurrentUserResponse?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new CurrentUserResponse(
                x.Id,
                x.Email,
                x.DisplayName))
            .FirstOrDefaultAsync(cancellationToken);
}
