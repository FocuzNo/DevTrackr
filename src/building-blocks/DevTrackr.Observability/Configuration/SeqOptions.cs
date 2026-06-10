namespace DevTrackr.Observability.Configuration;

public sealed class SeqOptions
{
    public const string SectionName = "Seq";

    public string? ServerUrl { get; init; }
}
