namespace DevTrackr.Observability.Configuration;

public sealed class OpenTelemetryOptions
{
    public const string SectionName = "OpenTelemetry";

    public string? OtlpEndpoint { get; init; }

    public string? ServiceName { get; init; }
}
