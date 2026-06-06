using DevTrackr.Cqrs.Abstractions;
using DevTrackr.Cqrs.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace DevTrackr.Cqrs.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCqrs(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (assemblies is null || assemblies.Length == 0)
        {
            throw new ArgumentException("At least one assembly must be provided for CQRS registration.", nameof(assemblies));
        }

        services.AddScoped<IAppMediator, AppMediator>();
        services.TryAddEnumerable(ServiceDescriptor.Scoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)));

        services.AddValidatorsFromAssemblies(assemblies);

        foreach (var assembly in assemblies.Distinct())
        {
            RegisterHandlers(services, assembly);
        }

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Select(type => new
            {
                ImplementationType = type,
                ServiceTypes = type.GetInterfaces()
                    .Where(@interface => @interface.IsGenericType)
                    .Where(@interface =>
                    {
                        var definition = @interface.GetGenericTypeDefinition();
                        return definition == typeof(ICommandHandler<>)
                            || definition == typeof(ICommandHandler<,>)
                            || definition == typeof(IQueryHandler<,>);
                    })
                    .ToArray()
            })
            .Where(x => x.ServiceTypes.Length > 0);

        foreach (var handlerType in handlerTypes)
        {
            foreach (var serviceType in handlerType.ServiceTypes)
            {
                services.AddScoped(serviceType, handlerType.ImplementationType);
            }
        }
    }
}
