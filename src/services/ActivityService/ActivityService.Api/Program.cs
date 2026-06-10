using ActivityService.Api.Extensions;
using ActivityService.Application;
using ActivityService.Infrastructure;
using ActivityService.Infrastructure.Persistence;
using DevTrackr.Observability.Extensions;
using DevTrackr.Security.Authentication;
using DevTrackr.Security.CurrentUser;
using DevTrackr.Security.OpenApi;
using FastEndpoints;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddDevTrackrObservability("ActivityService");
builder.Services.AddDevTrackrOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddFastEndpoints();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCurrentUser();

var app = builder.Build();

if (app.Environment.ShouldApplyMigrations())
{
    await app.ApplyMigrationsAsync<ActivityDbContext>();
}

app.UseDevTrackrObservability("ActivityService");
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.MapScalarApiReference(
    "/scalar/v1",
    options => options
        .WithTitle("ActivityService API")
        .AddPreferredSecuritySchemes("Bearer"));
app.MapHealthChecks("/health");
app.MapGet("/api/system/ping", () => Results.Ok(new
{
    Service = "ActivityService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));

if (app.Environment.IsDevelopment())
{
    app.MapGet(
        "/api/system/error",
        (HttpContext _) => throw new InvalidOperationException("Development exception test for ActivityService."));
}

app.UseFastEndpoints();

app.Run();
