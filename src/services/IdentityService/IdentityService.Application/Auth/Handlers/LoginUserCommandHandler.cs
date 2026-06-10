using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using IdentityService.Application.Abstractions.Persistence;
using IdentityService.Application.Abstractions.Security;
using IdentityService.Application.Auth.Commands;
using IdentityService.Application.Auth.Responses;
using IdentityService.Domain.Users;

namespace IdentityService.Application.Auth.Handlers;

public sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator)
    : ICommandHandler<LoginUserCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> HandleAsync(
        LoginUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByNormalizedEmailAsync(
            User.NormalizeEmail(command.Email),
            cancellationToken);

        if (user is null)
        {
            return Result<AuthResponse>.Failure(IdentityErrors.InvalidCredentials);
        }

        var isValidPassword = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            command.Password);

        if (!isValidPassword)
        {
            return Result<AuthResponse>.Failure(IdentityErrors.InvalidCredentials);
        }

        var token = jwtTokenGenerator.Generate(user);
        return Result<AuthResponse>.Success(
            user.ToAuthResponse(
                token.AccessToken,
                token.ExpiresAt));
    }
}
