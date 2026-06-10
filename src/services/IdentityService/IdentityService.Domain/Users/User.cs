using DevTrackr.SharedKernel.Primitives;

namespace IdentityService.Domain.Users;

public sealed class User : AggregateRoot<Guid>
{
    private User()
        : base(Guid.Empty)
    {
        Email = string.Empty;
        NormalizedEmail = string.Empty;
        PasswordHash = string.Empty;
        DisplayName = string.Empty;
    }

    private User(
        Guid id,
        string email,
        string normalizedEmail,
        string passwordHash,
        string displayName,
        DateTime createdAt,
        DateTime updatedAt)
        : base(id)
    {
        Email = email;
        NormalizedEmail = normalizedEmail;
        PasswordHash = passwordHash;
        DisplayName = displayName;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public string Email { get; private set; }

    public string NormalizedEmail { get; private set; }

    public string PasswordHash { get; private set; }

    public string DisplayName { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public static Result<User> Create(
        string email,
        string displayName,
        string passwordHash,
        DateTime utcNow)
    {
        var trimmedEmail = email.Trim();
        if (string.IsNullOrWhiteSpace(trimmedEmail))
        {
            return Result<User>.Failure(IdentityErrors.EmailRequired);
        }

        var trimmedDisplayName = displayName.Trim();
        if (string.IsNullOrWhiteSpace(trimmedDisplayName))
        {
            return Result<User>.Failure(IdentityErrors.DisplayNameRequired);
        }

        if (trimmedDisplayName.Length > 100)
        {
            return Result<User>.Failure(IdentityErrors.DisplayNameTooLong);
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result<User>.Failure(IdentityErrors.PasswordHashRequired);
        }

        return Result<User>.Success(
            new User(
                Guid.NewGuid(),
                trimmedEmail.ToLowerInvariant(),
                NormalizeEmail(trimmedEmail),
                passwordHash,
                trimmedDisplayName,
                utcNow,
                utcNow));
    }

    public static string NormalizeEmail(string email) => email.Trim().ToUpperInvariant();
}
