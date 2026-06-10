using DevTrackr.Cqrs.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddCqrs(typeof(DependencyInjection).Assembly);
        return services;
    }
}
