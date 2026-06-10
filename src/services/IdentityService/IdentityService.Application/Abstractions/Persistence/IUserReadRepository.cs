using IdentityService.Application.Users.Responses;

namespace IdentityService.Application.Abstractions.Persistence;

public interface IUserReadRepository
{
    Task<CurrentUserResponse?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
