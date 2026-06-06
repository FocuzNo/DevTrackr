using IdentityService.Application;
using IdentityService.Application.Auth;
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
app.MapScalarApiReference("/scalar/v1", options => options.WithTitle("IdentityService API"));
app.MapHealthChecks("/health");
app.MapGet("/api/system/ping", () => Results.Ok(new
{
    Service = "IdentityService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));

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
