using IdentityService.Domain.Users;
using IdentityService.Infrastructure.Authentication;
using Xunit;

namespace IdentityService.UnitTests;

public sealed class PasswordHasherTests
{
    [Fact]
    public void VerifyHashedPassword_ValidPassword_ReturnsTrue()
    {
        var user = CreateUser();
        var passwordHasher = new PasswordHasher();
        var hashedPassword = passwordHasher.HashPassword(user, "secret-pass");

        var isValid = passwordHasher.VerifyHashedPassword(user, hashedPassword, "secret-pass");

        Assert.True(isValid);
    }

    [Fact]
    public void VerifyHashedPassword_InvalidPassword_ReturnsFalse()
    {
        var user = CreateUser();
        var passwordHasher = new PasswordHasher();
        var hashedPassword = passwordHasher.HashPassword(user, "secret-pass");

        var isValid = passwordHasher.VerifyHashedPassword(user, hashedPassword, "other-pass");

        Assert.False(isValid);
    }

    private static User CreateUser() =>
        User.Create(
            "user@example.com",
            "Dev User",
            "hashed-password",
            DateTime.UtcNow).Value!;
}
