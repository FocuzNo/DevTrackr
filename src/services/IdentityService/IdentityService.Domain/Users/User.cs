using DevTrackr.SharedKernel.Primitives;

namespace IdentityService.Domain.Users;

public sealed class User : AggregateRoot<Guid>
{
    private User(Guid id, string email, string displayName, string passwordHash)
        : base(id)
    {
        Email = email;
        DisplayName = displayName;
        PasswordHash = passwordHash;
    }

    public string Email { get; private set; }

    public string DisplayName { get; private set; }

    public string PasswordHash { get; private set; }

    public static User Create(string email, string displayName, string passwordHash) =>
        new(Guid.NewGuid(), email.Trim(), displayName.Trim(), passwordHash);
}
