using ActivityService.Api.Auth;
using ActivityService.Api.Extensions;
using ActivityService.Application;
using ActivityService.Infrastructure;
using ActivityService.Infrastructure.Persistence;
using DevTrackr.Observability.Extensions;
using FastEndpoints;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddDevTrackrObservability("ActivityService");
builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddFastEndpoints();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<CurrentUserOptions>(builder.Configuration.GetSection(CurrentUserOptions.SectionName));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

var app = builder.Build();

if (app.Environment.ShouldApplyMigrations())
{
    await app.ApplyMigrationsAsync<ActivityDbContext>();
}

app.UseDevTrackrObservability("ActivityService");
app.MapOpenApi();
app.MapScalarApiReference("/scalar/v1", options => options.WithTitle("ActivityService API"));
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
