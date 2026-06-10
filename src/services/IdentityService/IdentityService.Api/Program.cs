using DevTrackr.Observability.Extensions;
using DevTrackr.Security.Authentication;
using DevTrackr.Security.CurrentUser;
using DevTrackr.Security.OpenApi;
using FastEndpoints;
using IdentityService.Api.Extensions;
using IdentityService.Application;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddDevTrackrObservability("IdentityService");
builder.Services.AddDevTrackrOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddFastEndpoints();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCurrentUser();

var app = builder.Build();

if (app.Environment.ShouldApplyMigrations())
{
    await app.ApplyMigrationsAsync<IdentityDbContext>();
}

app.UseDevTrackrObservability("IdentityService");
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.MapScalarApiReference(
    "/scalar/v1",
    options => options
        .WithTitle("IdentityService API")
        .AddPreferredSecuritySchemes("Bearer"));
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

app.UseFastEndpoints();

app.Run();
