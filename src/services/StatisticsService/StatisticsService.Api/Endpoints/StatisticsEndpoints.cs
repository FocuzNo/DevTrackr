using DevTrackr.Cqrs.Abstractions;
using FastEndpoints;
using StatisticsService.Api.Auth;
using StatisticsService.Api.Extensions;
using StatisticsService.Application.Statistics.Queries;

namespace StatisticsService.Api.Endpoints;

public sealed class GetDashboardEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/statistics/dashboard");
        AllowAnonymous();
        Summary(s => s.Summary = "Get dashboard statistics for the current user.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetDashboardQuery(currentUserProvider.GetRequiredUserId()),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class GetWeeklyStatisticsEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/statistics/weekly");
        AllowAnonymous();
        Summary(s => s.Summary = "Get weekly study statistics.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetWeeklyStatisticsQuery(currentUserProvider.GetRequiredUserId()),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class GetTopicStatisticsEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/statistics/topics");
        AllowAnonymous();
        Summary(s => s.Summary = "Get topic statistics.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetTopicStatisticsQuery(currentUserProvider.GetRequiredUserId()),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class GetStreakEndpoint(IAppMediator mediator, ICurrentUserProvider currentUserProvider)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/statistics/streak");
        AllowAnonymous();
        Summary(s => s.Summary = "Get streak statistics.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetStreakQuery(currentUserProvider.GetRequiredUserId()),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}
