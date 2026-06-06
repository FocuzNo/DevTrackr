using FastEndpoints;
using Scalar.AspNetCore;
using Serilog;
using StatisticsService.Api.Auth;
using StatisticsService.Api.Extensions;
using StatisticsService.Application;
using StatisticsService.Infrastructure;
using StatisticsService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .WriteTo.Console());

builder.Services.AddOpenApi();
builder.Services.AddFastEndpoints();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<CurrentUserOptions>(builder.Configuration.GetSection(CurrentUserOptions.SectionName));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

var app = builder.Build();

if (app.Environment.ShouldApplyMigrations())
{
    await app.ApplyMigrationsAsync<StatisticsDbContext>();
}

app.MapOpenApi();
app.MapScalarApiReference("/scalar/v1", options => options.WithTitle("StatisticsService API"));
app.MapHealthChecks("/health");
app.MapGet("/api/system/ping", () => Results.Ok(new
{
    Service = "StatisticsService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));
app.UseFastEndpoints();

app.Run();
