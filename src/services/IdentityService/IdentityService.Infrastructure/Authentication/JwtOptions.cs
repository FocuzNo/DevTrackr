namespace IdentityService.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "DevTrackr";

    public string Audience { get; init; } = "DevTrackr";

    public string SigningKey { get; init; } = "devtrackr-super-secret-signing-key";

    public int ExpirationMinutes { get; init; } = 60;
}
