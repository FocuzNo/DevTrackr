namespace DevTrackr.Security.CurrentUser;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? Email { get; }

    bool IsAuthenticated { get; }

    Guid GetRequiredUserId();
}
