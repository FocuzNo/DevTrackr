using ActivityService.Application;
using ActivityService.Application.Sessions;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .WriteTo.Console());

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddApplication();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference("/scalar/v1", options => options.WithTitle("ActivityService API"));
app.MapHealthChecks("/health");
app.MapGet("/api/system/ping", () => Results.Ok(new
{
    Service = "ActivityService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));

var api = app.MapGroup("/api/activity").WithTags("Activity");

api.MapGet("/test", () => Results.Ok(new
{
    Service = "ActivityService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));

api.MapPost("/sessions", (LogStudySessionRequest request) =>
    Results.Accepted(value: new
    {
        Message = "Log study session endpoint scaffolded.",
        Request = request
    }));

api.MapGet("/goals/{goalId:guid}/sessions", (Guid goalId) =>
    Results.Ok(new
    {
        GoalId = goalId,
        Sessions = Array.Empty<object>()
    }));

api.MapGet("/sessions", (DateOnly? from, DateOnly? to) =>
    Results.Ok(new
    {
        From = from,
        To = to,
        Sessions = Array.Empty<object>()
    }));

app.Run();
