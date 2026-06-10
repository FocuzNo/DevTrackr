using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using IdentityService.Application.Abstractions.Persistence;
using IdentityService.Application.Abstractions.Security;
using IdentityService.Application.Auth.Commands;
using IdentityService.Application.Auth.Responses;
using IdentityService.Domain.Users;

namespace IdentityService.Application.Auth.Handlers;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RegisterUserCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = User.NormalizeEmail(command.Email);
        var emailExists = await userRepository.ExistsByNormalizedEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (emailExists)
        {
            return Result<AuthResponse>.Failure(IdentityErrors.EmailAlreadyExists);
        }

        var placeholderUser = User.Create(
            command.Email,
            command.DisplayName,
            "temporary-password-hash",
            DateTime.UtcNow);

        if (placeholderUser.IsFailure || placeholderUser.Value is null)
        {
            return Result<AuthResponse>.Failure(placeholderUser.Error);
        }

        var passwordHash = passwordHasher.HashPassword(placeholderUser.Value, command.Password);
        var userResult = User.Create(
            command.Email,
            command.DisplayName,
            passwordHash,
            DateTime.UtcNow);

        if (userResult.IsFailure || userResult.Value is null)
        {
            return Result<AuthResponse>.Failure(userResult.Error);
        }

        await userRepository.AddAsync(userResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var token = jwtTokenGenerator.Generate(userResult.Value);
        return Result<AuthResponse>.Success(
            userResult.Value.ToAuthResponse(
                token.AccessToken,
                token.ExpiresAt));
    }
}
