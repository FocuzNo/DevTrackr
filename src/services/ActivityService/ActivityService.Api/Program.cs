using ActivityService.Api.Auth;
using ActivityService.Api.Extensions;
using ActivityService.Application;
using ActivityService.Application.Abstractions;
using ActivityService.Application.Sessions.Commands;
using ActivityService.Application.Sessions.Queries;
using ActivityService.Application.Sessions.Requests;
using ActivityService.Application.Sessions.Responses;
using ActivityService.Infrastructure;
using ActivityService.Infrastructure.Persistence;
using DevTrackr.SharedKernel.Primitives;
using Scalar.AspNetCore;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .WriteTo.Console());

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
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

app.MapOpenApi();
app.MapScalarApiReference("/scalar/v1", options => options.WithTitle("ActivityService API"));
app.MapHealthChecks("/health");
app.MapGet("/api/system/ping", () => Results.Ok(new
{
    Service = "ActivityService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));

var api = app.MapGroup("/api/study-sessions").WithTags("Study Sessions");

api.MapPost("/", async (
        LogStudySessionRequest request,
        ICurrentUserProvider currentUserProvider,
        ICommandHandler<LogStudySessionCommand, Result<StudySessionResponse>> handler,
        CancellationToken cancellationToken) =>
    {
        var command = new LogStudySessionCommand(
            currentUserProvider.GetRequiredUserId(),
            request.GoalId,
            request.Topic,
            request.DurationMinutes,
            request.Difficulty,
            request.Note,
            request.SessionDate);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToApiResult(response => Results.Created($"/api/study-sessions/{response.Id}", response));
    })
    .WithSummary("Log a new study session.");

api.MapGet("/", async (
        ICurrentUserProvider currentUserProvider,
        IQueryHandler<GetStudySessionsQuery, IReadOnlyList<StudySessionListItemResponse>> handler,
        CancellationToken cancellationToken) =>
    {
        var response = await handler.HandleAsync(
            new GetStudySessionsQuery(currentUserProvider.GetRequiredUserId()),
            cancellationToken);

        return Results.Ok(response);
    })
    .WithSummary("Get all study sessions for the current user.");

api.MapGet("/{id:guid}", async (
        Guid id,
        ICurrentUserProvider currentUserProvider,
        IQueryHandler<GetStudySessionByIdQuery, Result<StudySessionResponse>> handler,
        CancellationToken cancellationToken) =>
    {
        var result = await handler.HandleAsync(
            new GetStudySessionByIdQuery(currentUserProvider.GetRequiredUserId(), id),
            cancellationToken);

        return result.ToApiResult(Results.Ok);
    })
    .WithSummary("Get a study session by id.");

api.MapGet("/by-goal/{goalId:guid}", async (
        Guid goalId,
        ICurrentUserProvider currentUserProvider,
        IQueryHandler<GetStudySessionsByGoalQuery, IReadOnlyList<StudySessionListItemResponse>> handler,
        CancellationToken cancellationToken) =>
    {
        var response = await handler.HandleAsync(
            new GetStudySessionsByGoalQuery(currentUserProvider.GetRequiredUserId(), goalId),
            cancellationToken);

        return Results.Ok(response);
    })
    .WithSummary("Get study sessions for a goal.");

api.MapGet("/by-date-range", async (
        DateOnly from,
        DateOnly to,
        ICurrentUserProvider currentUserProvider,
        IQueryHandler<GetStudySessionsByDateRangeQuery, Result<IReadOnlyList<StudySessionListItemResponse>>> handler,
        CancellationToken cancellationToken) =>
    {
        var result = await handler.HandleAsync(
            new GetStudySessionsByDateRangeQuery(currentUserProvider.GetRequiredUserId(), from, to),
            cancellationToken);

        return result.ToApiResult();
    })
    .WithSummary("Get study sessions by date range.");

app.Run();
