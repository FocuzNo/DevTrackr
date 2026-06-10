using IdentityService.Application.Abstractions.Persistence;
using IdentityService.Application.Abstractions.Security;
using IdentityService.Infrastructure.Authentication;
using IdentityService.Infrastructure.Persistence;
using IdentityService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("IdentityService database connection string is missing.");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                x => x.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserReadRepository, UserReadRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddHealthChecks();

        return services;
    }
}
