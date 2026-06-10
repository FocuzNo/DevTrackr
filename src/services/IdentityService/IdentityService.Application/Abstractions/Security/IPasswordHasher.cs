using IdentityService.Domain.Users;

namespace IdentityService.Application.Abstractions.Security;

public interface IPasswordHasher
{
    string HashPassword(
        User user,
        string password);

    bool VerifyHashedPassword(
        User user,
        string hashedPassword,
        string providedPassword);
}
