namespace IdentityService.Application.Auth.Requests;

public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string DisplayName);
