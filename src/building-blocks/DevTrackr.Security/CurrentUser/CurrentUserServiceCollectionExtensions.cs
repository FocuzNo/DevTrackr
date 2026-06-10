using Microsoft.Extensions.DependencyInjection;

namespace DevTrackr.Security.CurrentUser;

public static class CurrentUserServiceCollectionExtensions
{
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}
