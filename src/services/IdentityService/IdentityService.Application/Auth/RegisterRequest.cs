namespace IdentityService.Application.Auth;

public sealed record RegisterRequest(string Email, string Password, string DisplayName);
