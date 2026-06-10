using DevTrackr.Cqrs.Abstractions;
using DevTrackr.Security.Authentication;
using DevTrackr.Security.CurrentUser;
using FastEndpoints;
using StatisticsService.Api.Extensions;
using StatisticsService.Application.Statistics.Queries;

namespace StatisticsService.Api.Endpoints;

public sealed class GetDashboardEndpoint(IAppMediator mediator, ICurrentUserService currentUserService)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/statistics/dashboard");
        Policies(SecurityPolicies.Authenticated);
        Summary(s => s.Summary = "Get dashboard statistics for the current user.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetDashboardQuery(currentUserService.GetRequiredUserId()),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class GetWeeklyStatisticsEndpoint(IAppMediator mediator, ICurrentUserService currentUserService)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/statistics/weekly");
        Policies(SecurityPolicies.Authenticated);
        Summary(s => s.Summary = "Get weekly study statistics.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetWeeklyStatisticsQuery(currentUserService.GetRequiredUserId()),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class GetTopicStatisticsEndpoint(IAppMediator mediator, ICurrentUserService currentUserService)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/statistics/topics");
        Policies(SecurityPolicies.Authenticated);
        Summary(s => s.Summary = "Get topic statistics.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetTopicStatisticsQuery(currentUserService.GetRequiredUserId()),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class GetStreakEndpoint(IAppMediator mediator, ICurrentUserService currentUserService)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/statistics/streak");
        Policies(SecurityPolicies.Authenticated);
        Summary(s => s.Summary = "Get streak statistics.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetStreakQuery(currentUserService.GetRequiredUserId()),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}
