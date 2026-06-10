namespace DevTrackr.Security.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "DevTrackr";

    public string Audience { get; init; } = "DevTrackr";

    public string Secret { get; init; } = "devtrackr-local-development-secret-key-change-me";

    public int ExpirationMinutes { get; init; } = 60;
}
