using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace DevTrackr.Security.CurrentUser;

public sealed class CurrentUserService(
    IHttpContextAccessor httpContextAccessor,
    IHostEnvironment environment,
    IOptions<CurrentUserOptions> options) : ICurrentUserService
{
    public Guid? UserId => ResolveUserId();

    public string? Email =>
        httpContextAccessor.HttpContext?.User.FindFirstValue("email")
        ?? httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true
        || ResolveDevelopmentUserId() is not null;

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

        return ResolveDevelopmentUserId();
    }

    private Guid? ResolveDevelopmentUserId() =>
        environment.IsDevelopment()
            ? options.Value.DevelopmentUserId
            : null;
}
