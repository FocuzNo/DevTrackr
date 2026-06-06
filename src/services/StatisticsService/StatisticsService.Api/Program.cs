using Scalar.AspNetCore;
using Serilog;
using StatisticsService.Application.Dashboard;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .WriteTo.Console());

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference("/scalar/v1", options => options.WithTitle("StatisticsService API"));
app.MapHealthChecks("/health");
app.MapGet("/api/system/ping", () => Results.Ok(new
{
    Service = "StatisticsService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));

var api = app.MapGroup("/api/statistics").WithTags("Statistics");

api.MapGet("/test", () => Results.Ok(new
{
    Service = "StatisticsService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));

api.MapGet("/dashboard/{userId:guid}", (Guid userId) =>
{
    var response = new DashboardResponse(
        userId,
        TotalSessions: 0,
        TotalMinutes: 0,
        CompletedGoals: 0,
        CurrentStreakDays: 0,
        TopTopics: Array.Empty<string>());

    return Results.Ok(response);
});

app.Run();
