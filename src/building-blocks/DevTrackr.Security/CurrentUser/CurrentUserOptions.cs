namespace DevTrackr.Security.CurrentUser;

public sealed class CurrentUserOptions
{
    public const string SectionName = "CurrentUser";

    public Guid? DevelopmentUserId { get; init; }
}
