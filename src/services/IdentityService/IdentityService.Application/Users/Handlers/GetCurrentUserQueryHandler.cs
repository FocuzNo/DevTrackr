using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using IdentityService.Application.Abstractions.Persistence;
using IdentityService.Application.Users.Queries;
using IdentityService.Application.Users.Responses;
using IdentityService.Domain.Users;

namespace IdentityService.Application.Users.Handlers;

public sealed class GetCurrentUserQueryHandler(
    IUserReadRepository userReadRepository)
    : IQueryHandler<GetCurrentUserQuery, Result<CurrentUserResponse>>
{
    public async Task<Result<CurrentUserResponse>> HandleAsync(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken = default)
    {
        var user = await userReadRepository.GetByIdAsync(query.UserId, cancellationToken);
        return user is null
            ? Result<CurrentUserResponse>.Failure(IdentityErrors.UserNotFound)
            : Result<CurrentUserResponse>.Success(user);
    }
}
