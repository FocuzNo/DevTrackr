using DevTrackr.Security.Authentication;
using IdentityService.Application.Abstractions.Persistence;
using IdentityService.Application.Abstractions.Security;
using IdentityService.Application.Auth.Commands;
using IdentityService.Application.Auth.Handlers;
using IdentityService.Domain.Users;
using IdentityService.Infrastructure.Authentication;
using Microsoft.Extensions.Options;
using Xunit;

namespace IdentityService.UnitTests;

public sealed class AuthCommandHandlerTests
{
    [Fact]
    public async Task RegisterUserCommand_CreatesUser_AndReturnsJwt()
    {
        var repository = new InMemoryUserRepository();
        var handler = CreateRegisterHandler(repository);

        var result = await handler.HandleAsync(
            new RegisterUserCommand(
                "user@example.com",
                "secret-pass",
                "Dev User"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(repository.Users);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.AccessToken));
    }

    [Fact]
    public async Task RegisterUserCommand_Fails_WhenEmailAlreadyExists()
    {
        var repository = new InMemoryUserRepository();
        repository.Seed(CreateUser("user@example.com", "Dev User"));
        var handler = CreateRegisterHandler(repository);

        var result = await handler.HandleAsync(
            new RegisterUserCommand(
                "user@example.com",
                "secret-pass",
                "Dev User"));

        Assert.True(result.IsFailure);
        Assert.Equal(IdentityErrors.EmailAlreadyExists, result.Error);
    }

    [Fact]
    public async Task LoginUserCommand_ReturnsJwt_ForValidCredentials()
    {
        var repository = new InMemoryUserRepository();
        var passwordHasher = new PasswordHasher();
        var user = CreateUser("user@example.com", "Dev User");
        var hashedUser = User.Create(
            user.Email,
            user.DisplayName,
            passwordHasher.HashPassword(user, "secret-pass"),
            DateTime.UtcNow).Value!;

        repository.Seed(hashedUser);

        var handler = new LoginUserCommandHandler(
            repository,
            passwordHasher,
            CreateJwtTokenGenerator());

        var result = await handler.HandleAsync(
            new LoginUserCommand(
                "user@example.com",
                "secret-pass"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.AccessToken));
    }

    [Fact]
    public async Task LoginUserCommand_Fails_ForInvalidCredentials()
    {
        var repository = new InMemoryUserRepository();
        var passwordHasher = new PasswordHasher();
        var user = CreateUser("user@example.com", "Dev User");
        var hashedUser = User.Create(
            user.Email,
            user.DisplayName,
            passwordHasher.HashPassword(user, "secret-pass"),
            DateTime.UtcNow).Value!;

        repository.Seed(hashedUser);

        var handler = new LoginUserCommandHandler(
            repository,
            passwordHasher,
            CreateJwtTokenGenerator());

        var result = await handler.HandleAsync(
            new LoginUserCommand(
                "user@example.com",
                "wrong-pass"));

        Assert.True(result.IsFailure);
        Assert.Equal(IdentityErrors.InvalidCredentials, result.Error);
    }

    private static RegisterUserCommandHandler CreateRegisterHandler(InMemoryUserRepository repository) =>
        new(
            repository,
            new PasswordHasher(),
            CreateJwtTokenGenerator(),
            new TestUnitOfWork());

    private static IJwtTokenGenerator CreateJwtTokenGenerator() =>
        new JwtTokenGenerator(
            Options.Create(
                new JwtOptions
                {
                    Issuer = "DevTrackr",
                    Audience = "DevTrackr",
                    Secret = "devtrackr-local-development-secret-key-change-me",
                    ExpirationMinutes = 60
                }));

    private static User CreateUser(string email, string displayName) =>
        User.Create(
            email,
            displayName,
            "hashed-password",
            DateTime.UtcNow).Value!;

    private sealed class InMemoryUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Users.FirstOrDefault(x => x.Id == id));

        public Task AddAsync(User entity, CancellationToken cancellationToken = default)
        {
            Users.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(User entity)
        {
        }

        public void Remove(User entity) => Users.Remove(entity);

        public Task<bool> ExistsByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
            Task.FromResult(Users.Any(x => x.NormalizedEmail == normalizedEmail));

        public Task<User?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
            Task.FromResult(Users.FirstOrDefault(x => x.NormalizedEmail == normalizedEmail));

        public void Seed(User user) => Users.Add(user);
    }

    private sealed class TestUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }
}
