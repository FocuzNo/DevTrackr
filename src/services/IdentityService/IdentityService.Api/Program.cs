using IdentityService.Application;
using IdentityService.Application.Auth;
using DevTrackr.Observability.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddDevTrackrObservability("IdentityService");
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddApplication();

var app = builder.Build();

app.UseDevTrackrObservability("IdentityService");
app.MapOpenApi();
app.MapScalarApiReference("/scalar/v1", options => options.WithTitle("IdentityService API"));
app.MapHealthChecks("/health");
app.MapGet("/api/system/ping", () => Results.Ok(new
{
    Service = "IdentityService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));

if (app.Environment.IsDevelopment())
{
    app.MapGet(
        "/api/system/error",
        (HttpContext _) => throw new InvalidOperationException("Development exception test for IdentityService."));
}

var api = app.MapGroup("/api/identity").WithTags("Identity");

api.MapGet("/test", () => Results.Ok(new
{
    Service = "IdentityService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));

api.MapPost("/register", (RegisterRequest request) =>
    Results.Accepted(value: new
    {
        Message = "Registration endpoint scaffolded.",
        request.Email,
        request.DisplayName
    }));

api.MapPost("/login", (LoginRequest request) =>
    Results.Ok(new
    {
        Message = "Login endpoint scaffolded.",
        Token = "placeholder-jwt-token",
        request.Email
    }));

api.MapGet("/me", () =>
    Results.Ok(new
    {
        UserId = Guid.Empty,
        Email = "current-user@devtrackr.local",
        DisplayName = "Current User"
    }));

app.Run();
