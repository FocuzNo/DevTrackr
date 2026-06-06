using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace StatisticsService.Api.Auth;

public sealed class CurrentUserProvider(
    IHttpContextAccessor httpContextAccessor,
    IWebHostEnvironment environment,
    IOptions<CurrentUserOptions> options) : ICurrentUserProvider
{
    public Guid GetRequiredUserId()
    {
        var principal = httpContextAccessor.HttpContext?.User;
        var value = principal?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal?.FindFirstValue("sub")
            ?? principal?.FindFirstValue("userId");

        if (Guid.TryParse(value, out var userId))
        {
            return userId;
        }

        if (environment.IsDevelopment() && options.Value.DevelopmentUserId.HasValue)
        {
            return options.Value.DevelopmentUserId.Value;
        }

        throw new InvalidOperationException("Current user id is not available from JWT claims.");
    }
}
