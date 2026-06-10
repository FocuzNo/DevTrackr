using IdentityService.Application.Abstractions.Security;
using IdentityService.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Infrastructure.Authentication;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(User user, string password) =>
        _passwordHasher.HashPassword(user, password);

    public bool VerifyHashedPassword(
        User user,
        string hashedPassword,
        string providedPassword) =>
        _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword)
        != PasswordVerificationResult.Failed;
}
