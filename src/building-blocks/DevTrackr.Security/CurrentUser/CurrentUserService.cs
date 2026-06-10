using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DevTrackr.Security.CurrentUser;

public sealed class CurrentUserService(
    IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId => ResolveUserId();

    public string? Email =>
        httpContextAccessor.HttpContext?.User.FindFirstValue("email")
        ?? httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public Guid GetRequiredUserId() =>
        UserId ?? throw new InvalidOperationException("Current user id is not available.");

    private Guid? ResolveUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            if (Guid.TryParse(user.FindFirstValue("sub"), out var userId))
            {
                return userId;
            }

            if (Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var nameIdentifier))
            {
                return nameIdentifier;
            }
        }

        return null;
    }
}
