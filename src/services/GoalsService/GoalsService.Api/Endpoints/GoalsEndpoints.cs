using DevTrackr.Cqrs.Abstractions;
using DevTrackr.Security.Authentication;
using DevTrackr.Security.CurrentUser;
using FastEndpoints;
using GoalsService.Api.Extensions;
using GoalsService.Application.Goals.Commands;
using GoalsService.Application.Goals.Queries;
using GoalsService.Application.Goals.Requests;

namespace GoalsService.Api.Endpoints;

public sealed class CreateGoalEndpoint(IAppMediator mediator, ICurrentUserService currentUserService)
    : Endpoint<CreateGoalRequest>
{
    public override void Configure()
    {
        Post("/api/goals");
        Policies(SecurityPolicies.Authenticated);
        Summary(s => s.Summary = "Create a new learning goal.");
    }

    public override async Task HandleAsync(CreateGoalRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateGoalCommand(
            currentUserService.GetRequiredUserId(),
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

public sealed class GetGoalsEndpoint(IAppMediator mediator, ICurrentUserService currentUserService)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/goals");
        Policies(SecurityPolicies.Authenticated);
        Summary(s => s.Summary = "Get all goals for the current user.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetGoalsQuery(currentUserService.GetRequiredUserId()),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class GetGoalByIdEndpoint(IAppMediator mediator, ICurrentUserService currentUserService)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/goals/{id:guid}");
        Policies(SecurityPolicies.Authenticated);
        Summary(s => s.Summary = "Get a goal by id.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetGoalByIdQuery(currentUserService.GetRequiredUserId(), Route<Guid>("id")),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class UpdateGoalEndpoint(IAppMediator mediator, ICurrentUserService currentUserService)
    : Endpoint<UpdateGoalRequest>
{
    public override void Configure()
    {
        Put("/api/goals/{id:guid}");
        Policies(SecurityPolicies.Authenticated);
        Summary(s => s.Summary = "Update goal details.");
    }

    public override async Task HandleAsync(UpdateGoalRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new UpdateGoalCommand(
                currentUserService.GetRequiredUserId(),
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

public sealed class CompleteGoalEndpoint(IAppMediator mediator, ICurrentUserService currentUserService)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/api/goals/{id:guid}/complete");
        Policies(SecurityPolicies.Authenticated);
        Summary(s => s.Summary = "Complete a goal.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new CompleteGoalCommand(currentUserService.GetRequiredUserId(), Route<Guid>("id")),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class CancelGoalEndpoint(IAppMediator mediator, ICurrentUserService currentUserService)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/api/goals/{id:guid}/cancel");
        Policies(SecurityPolicies.Authenticated);
        Summary(s => s.Summary = "Cancel a goal.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new CancelGoalCommand(currentUserService.GetRequiredUserId(), Route<Guid>("id")),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}
