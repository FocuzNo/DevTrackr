using FastEndpoints;
using DevTrackr.Observability.Extensions;
using Scalar.AspNetCore;
using StatisticsService.Api.Auth;
using StatisticsService.Api.Extensions;
using StatisticsService.Application;
using StatisticsService.Infrastructure;
using StatisticsService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddDevTrackrObservability("StatisticsService");
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

app.UseDevTrackrObservability("StatisticsService");
app.MapOpenApi();
app.MapScalarApiReference("/scalar/v1", options => options.WithTitle("StatisticsService API"));
app.MapHealthChecks("/health");
app.MapGet("/api/system/ping", () => Results.Ok(new
{
    Service = "StatisticsService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));

if (app.Environment.IsDevelopment())
{
    app.MapGet(
        "/api/system/error",
        (HttpContext _) => throw new InvalidOperationException("Development exception test for StatisticsService."));
}

app.UseFastEndpoints();

app.Run();
