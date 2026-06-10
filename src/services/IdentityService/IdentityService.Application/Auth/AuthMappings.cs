using IdentityService.Application.Auth.Responses;
using IdentityService.Application.Users.Responses;
using IdentityService.Domain.Users;

namespace IdentityService.Application.Auth;

public static class AuthMappings
{
    public static CurrentUserResponse ToCurrentUserResponse(this User user) =>
        new(
            user.Id,
            user.Email,
            user.DisplayName);

    public static AuthResponse ToAuthResponse(
        this User user,
        string accessToken,
        DateTime expiresAt) =>
        new(
            accessToken,
            expiresAt,
            user.ToCurrentUserResponse());
}
