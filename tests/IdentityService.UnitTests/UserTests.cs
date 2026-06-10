using IdentityService.Domain.Users;
using Xunit;

namespace IdentityService.UnitTests;

public sealed class UserTests
{
    [Fact]
    public void Create_ValidUser_ReturnsSuccess()
    {
        var result = User.Create(
            "user@example.com",
            "Dev User",
            "hashed-password",
            DateTime.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("user@example.com", result.Value!.Email);
    }

    [Fact]
    public void Create_EmptyEmail_ReturnsFailure()
    {
        var result = User.Create(
            string.Empty,
            "Dev User",
            "hashed-password",
            DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(IdentityErrors.EmailRequired, result.Error);
    }

    [Fact]
    public void Create_EmptyDisplayName_ReturnsFailure()
    {
        var result = User.Create(
            "user@example.com",
            string.Empty,
            "hashed-password",
            DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(IdentityErrors.DisplayNameRequired, result.Error);
    }

    [Fact]
    public void NormalizeEmail_UppercasesAndTrims()
    {
        var normalizedEmail = User.NormalizeEmail("  User@Example.com ");

        Assert.Equal("USER@EXAMPLE.COM", normalizedEmail);
    }
}
