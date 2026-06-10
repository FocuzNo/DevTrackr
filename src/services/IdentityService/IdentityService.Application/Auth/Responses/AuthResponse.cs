using IdentityService.Application.Users.Responses;

namespace IdentityService.Application.Auth.Responses;

public sealed record AuthResponse(
    string AccessToken,
    DateTime ExpiresAt,
    CurrentUserResponse User);
