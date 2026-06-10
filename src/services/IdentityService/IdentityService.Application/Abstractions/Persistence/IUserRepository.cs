using DevTrackr.SharedKernel.Persistence;
using IdentityService.Domain.Users;

namespace IdentityService.Application.Abstractions.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task<bool> ExistsByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default);

    Task<User?> GetByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default);
}
