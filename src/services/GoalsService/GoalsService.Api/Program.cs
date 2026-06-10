using FastEndpoints;
using DevTrackr.Observability.Extensions;
using GoalsService.Api.Auth;
using GoalsService.Api.Extensions;
using GoalsService.Application;
using GoalsService.Infrastructure;
using GoalsService.Infrastructure.Persistence;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddDevTrackrObservability("GoalsService");
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
    await app.ApplyMigrationsAsync<GoalsDbContext>();
}

app.UseDevTrackrObservability("GoalsService");
app.MapOpenApi();
app.MapScalarApiReference("/scalar/v1", options => options.WithTitle("GoalsService API"));
app.MapHealthChecks("/health");
app.MapGet("/api/system/ping", () => Results.Ok(new
{
    Service = "GoalsService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));

if (app.Environment.IsDevelopment())
{
    app.MapGet(
        "/api/system/error",
        (HttpContext _) => throw new InvalidOperationException("Development exception test for GoalsService."));
}

app.UseFastEndpoints();

app.Run();
