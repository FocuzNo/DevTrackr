namespace IdentityService.Application.Users.Responses;

public sealed record CurrentUserResponse(
    Guid Id,
    string Email,
    string DisplayName);
