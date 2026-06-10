using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using IdentityService.Application.Auth.Responses;

namespace IdentityService.Application.Auth.Commands;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string DisplayName) : ICommand<Result<AuthResponse>>;
