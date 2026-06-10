using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevTrackr.Security.CurrentUser;

public static class CurrentUserServiceCollectionExtensions
{
    public static IServiceCollection AddCurrentUser(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CurrentUserOptions>(configuration.GetSection(CurrentUserOptions.SectionName));
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}
