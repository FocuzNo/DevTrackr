namespace IdentityService.Application.Abstractions.Security;

public sealed record AccessTokenResult(
    string AccessToken,
    DateTime ExpiresAt);
