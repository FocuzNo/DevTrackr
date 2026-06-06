using ActivityService.Api.Auth;
using ActivityService.Api.Extensions;
using ActivityService.Application.Sessions.Commands;
using ActivityService.Application.Sessions.Queries;
using ActivityService.Application.Sessions.Requests;
using DevTrackr.Cqrs.Abstractions;
using FastEndpoints;

namespace ActivityService.Api.Endpoints;

public sealed class LogStudySessionEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : Endpoint<LogStudySessionRequest>
{
    public override void Configure()
    {
        Post("/api/study-sessions");
        AllowAnonymous();
        Summary(s => s.Summary = "Log a new study session.");
    }

    public override async Task HandleAsync(LogStudySessionRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new LogStudySessionCommand(
                currentUserProvider.GetRequiredUserId(),
                request.GoalId,
                request.Topic,
                request.DurationMinutes,
                request.Difficulty,
                request.Note,
                request.SessionDate),
            cancellationToken);

        await result
            .ToApiResult(response => Results.Created($"/api/study-sessions/{response.Id}", response))
            .ExecuteAsync(HttpContext);
    }
}

public sealed class GetStudySessionsEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/study-sessions");
        AllowAnonymous();
        Summary(s => s.Summary = "Get all study sessions for the current user.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetStudySessionsQuery(currentUserProvider.GetRequiredUserId()),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class GetStudySessionByIdEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/study-sessions/{id:guid}");
        AllowAnonymous();
        Summary(s => s.Summary = "Get a study session by id.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetStudySessionByIdQuery(currentUserProvider.GetRequiredUserId(), Route<Guid>("id")),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class GetStudySessionsByGoalEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/study-sessions/by-goal/{goalId:guid}");
        AllowAnonymous();
        Summary(s => s.Summary = "Get study sessions for a goal.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetStudySessionsByGoalQuery(currentUserProvider.GetRequiredUserId(), Route<Guid>("goalId")),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class GetStudySessionsByDateRangeEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/study-sessions/by-date-range");
        AllowAnonymous();
        Summary(s => s.Summary = "Get study sessions by date range.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetStudySessionsByDateRangeQuery(
                currentUserProvider.GetRequiredUserId(),
                Query<DateOnly>("from"),
                Query<DateOnly>("to")),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}
