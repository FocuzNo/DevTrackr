using FastEndpoints;
using DevTrackr.Observability.Extensions;
using DevTrackr.Security.Authentication;
using DevTrackr.Security.CurrentUser;
using DevTrackr.Security.OpenApi;
using Scalar.AspNetCore;
using StatisticsService.Api.Extensions;
using StatisticsService.Application;
using StatisticsService.Infrastructure;
using StatisticsService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddDevTrackrObservability("StatisticsService");
builder.Services.AddDevTrackrOpenApi();
builder.Services.AddFastEndpoints();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCurrentUser();

var app = builder.Build();

if (app.Environment.ShouldApplyMigrations())
{
    await app.ApplyMigrationsAsync<StatisticsDbContext>();
}

app.UseDevTrackrObservability("StatisticsService");
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.MapScalarApiReference(
    "/scalar/v1",
    options => options
        .WithTitle("StatisticsService API")
        .AddPreferredSecuritySchemes("Bearer"));
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
