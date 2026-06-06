namespace ActivityService.Api.Auth;

public sealed class CurrentUserOptions
{
    public const string SectionName = "CurrentUser";

    public Guid? DevelopmentUserId { get; init; }
}
