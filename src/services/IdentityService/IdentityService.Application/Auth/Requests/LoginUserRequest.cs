namespace IdentityService.Application.Auth.Requests;

public sealed record LoginUserRequest(
    string Email,
    string Password);
