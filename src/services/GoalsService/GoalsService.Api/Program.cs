using DevTrackr.SharedKernel.Primitives;
using GoalsService.Api.Auth;
using GoalsService.Api.Extensions;
using GoalsService.Application;
using GoalsService.Application.Abstractions;
using GoalsService.Application.Goals.Commands;
using GoalsService.Application.Goals.Queries;
using GoalsService.Application.Goals.Requests;
using GoalsService.Application.Goals.Responses;
using GoalsService.Infrastructure;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .WriteTo.Console());

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<CurrentUserOptions>(builder.Configuration.GetSection(CurrentUserOptions.SectionName));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

var app = builder.Build();

if (app.Environment.ShouldApplyMigrations())
{
    await app.ApplyMigrationsAsync<GoalsService.Infrastructure.Persistence.GoalsDbContext>();
}

app.MapOpenApi();
app.MapScalarApiReference("/scalar/v1", options => options.WithTitle("GoalsService API"));
app.MapHealthChecks("/health");
app.MapGet("/api/system/ping", () => Results.Ok(new
{
    Service = "GoalsService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));

var api = app.MapGroup("/api/goals").WithTags("Goals");

api.MapGet("/test", () => Results.Ok(new
{
    Service = "GoalsService",
    Status = "Running",
    UtcNow = DateTime.UtcNow
}));

api.MapPost("/", async (
        CreateGoalRequest request,
        ICurrentUserProvider currentUserProvider,
        ICommandHandler<CreateGoalCommand, Result<GoalResponse>> handler,
        CancellationToken cancellationToken) =>
    {
        var command = new CreateGoalCommand(
            currentUserProvider.GetRequiredUserId(),
            request.Title,
            request.Description,
            request.Category,
            request.TargetMinutes,
            request.StartDate,
            request.Deadline);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToApiResult(response => Results.Created($"/api/goals/{response.Id}", response));
    })
    .WithSummary("Create a new learning goal.");

api.MapGet("/", async (
        ICurrentUserProvider currentUserProvider,
        IQueryHandler<GetGoalsQuery, IReadOnlyList<GoalListItemResponse>> handler,
        CancellationToken cancellationToken) =>
    {
        var response = await handler.HandleAsync(
            new GetGoalsQuery(currentUserProvider.GetRequiredUserId()),
            cancellationToken);

        return Results.Ok(response);
    })
    .WithSummary("Get all goals for the current user.");

api.MapGet("/{id:guid}", async (
        Guid id,
        ICurrentUserProvider currentUserProvider,
        IQueryHandler<GetGoalByIdQuery, Result<GoalResponse>> handler,
        CancellationToken cancellationToken) =>
    {
        var result = await handler.HandleAsync(
            new GetGoalByIdQuery(currentUserProvider.GetRequiredUserId(), id),
            cancellationToken);

        return result.ToApiResult(Results.Ok);
    })
    .WithSummary("Get a goal by id.");

api.MapPut("/{id:guid}", async (
        Guid id,
        UpdateGoalRequest request,
        ICurrentUserProvider currentUserProvider,
        ICommandHandler<UpdateGoalCommand, Result<GoalResponse>> handler,
        CancellationToken cancellationToken) =>
    {
        var result = await handler.HandleAsync(
            new UpdateGoalCommand(
                currentUserProvider.GetRequiredUserId(),
                id,
                request.Title,
                request.Description,
                request.Category,
                request.TargetMinutes,
                request.StartDate,
                request.Deadline),
            cancellationToken);

        return result.ToApiResult(Results.Ok);
    })
    .WithSummary("Update goal details.");

api.MapPost("/{id:guid}/complete", async (
        Guid id,
        ICurrentUserProvider currentUserProvider,
        ICommandHandler<CompleteGoalCommand, Result<GoalResponse>> handler,
        CancellationToken cancellationToken) =>
    {
        var result = await handler.HandleAsync(
            new CompleteGoalCommand(currentUserProvider.GetRequiredUserId(), id),
            cancellationToken);

        return result.ToApiResult(Results.Ok);
    })
    .WithSummary("Complete a goal.");

api.MapPost("/{id:guid}/cancel", async (
        Guid id,
        ICurrentUserProvider currentUserProvider,
        ICommandHandler<CancelGoalCommand, Result<GoalResponse>> handler,
        CancellationToken cancellationToken) =>
    {
        var result = await handler.HandleAsync(
            new CancelGoalCommand(currentUserProvider.GetRequiredUserId(), id),
            cancellationToken);

        return result.ToApiResult(Results.Ok);
    })
    .WithSummary("Cancel a goal.");

app.Run();
