using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using IdentityService.Application.Auth.Responses;

namespace IdentityService.Application.Auth.Commands;

public sealed record LoginUserCommand(
    string Email,
    string Password) : ICommand<Result<AuthResponse>>;
