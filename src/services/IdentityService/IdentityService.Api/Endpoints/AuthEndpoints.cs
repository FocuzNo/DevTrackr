using DevTrackr.Cqrs.Abstractions;
using DevTrackr.Security.Authentication;
using DevTrackr.Security.CurrentUser;
using FastEndpoints;
using IdentityService.Api.Extensions;
using IdentityService.Application.Auth.Commands;
using IdentityService.Application.Auth.Requests;
using IdentityService.Application.Users.Queries;

namespace IdentityService.Api.Endpoints;

public sealed class RegisterUserEndpoint(IAppMediator mediator) : Endpoint<RegisterUserRequest>
{
    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
        Summary(s => s.Summary = "Register a new user.");
    }

    public override async Task HandleAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new RegisterUserCommand(
                request.Email,
                request.Password,
                request.DisplayName),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class LoginUserEndpoint(IAppMediator mediator) : Endpoint<LoginUserRequest>
{
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
        Summary(s => s.Summary = "Authenticate a user.");
    }

    public override async Task HandleAsync(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new LoginUserCommand(
                request.Email,
                request.Password),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}

public sealed class GetCurrentUserEndpoint(
    IAppMediator mediator,
    ICurrentUserService currentUserService) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/users/me");
        Policies(SecurityPolicies.Authenticated);
        Summary(s => s.Summary = "Get the current user.");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(
            new GetCurrentUserQuery(currentUserService.GetRequiredUserId()),
            cancellationToken);

        await result.ToApiResult().ExecuteAsync(HttpContext);
    }
}
