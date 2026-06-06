using DevTrackr.Cqrs.Abstractions;
using FastEndpoints;
using GoalsService.Api.Auth;
using GoalsService.Api.Extensions;
using GoalsService.Application.Goals.Commands;
using GoalsService.Application.Goals.Queries;
using GoalsService.Application.Goals.Requests;

namespace GoalsService.Api.Endpoints;

public sealed class CreateGoalEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : Endpoint<CreateGoalRequest>
{
    public override void Configure()
    {
        Post("/api/goals");
        AllowAnonymous();
        Summary(s => s.Summary = "Create a new learning goal.");
    }

    public override async Task HandleAsync(CreateGoalRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateGoalCommand(
            currentUserProvider.GetRequiredUserId(),
            request.Title,
            request.Description,
            request.Category,
            request.TargetMinutes,
            request.StartDate,
            request.Deadline);

        var result = await mediator.SendAsync(command, cancellationToken);
        await result.ToApiResult(response => Results.Created($"/api/goals/{response.Id}", response)).ExecuteAsync(HttpContext);
    }
}

public sealed class GetGoalsEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/goals");
        AllowAnonymous();
        Summary(s => s.Summary = "Get all goals for the current user.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetGoalsQuery(currentUserProvider.GetRequiredUserId()),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class GetGoalByIdEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/goals/{id:guid}");
        AllowAnonymous();
        Summary(s => s.Summary = "Get a goal by id.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetGoalByIdQuery(currentUserProvider.GetRequiredUserId(), Route<Guid>("id")),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class UpdateGoalEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : Endpoint<UpdateGoalRequest>
{
    public override void Configure()
    {
        Put("/api/goals/{id:guid}");
        AllowAnonymous();
        Summary(s => s.Summary = "Update goal details.");
    }

    public override async Task HandleAsync(UpdateGoalRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new UpdateGoalCommand(
                currentUserProvider.GetRequiredUserId(),
                Route<Guid>("id"),
                request.Title,
                request.Description,
                request.Category,
                request.TargetMinutes,
                request.StartDate,
                request.Deadline),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class CompleteGoalEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/api/goals/{id:guid}/complete");
        AllowAnonymous();
        Summary(s => s.Summary = "Complete a goal.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new CompleteGoalCommand(currentUserProvider.GetRequiredUserId(), Route<Guid>("id")),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class CancelGoalEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/api/goals/{id:guid}/cancel");
        AllowAnonymous();
        Summary(s => s.Summary = "Cancel a goal.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new CancelGoalCommand(currentUserProvider.GetRequiredUserId(), Route<Guid>("id")),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}
