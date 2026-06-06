namespace StatisticsService.Application.Statistics.Responses;

public sealed record StreakResponse(
    int CurrentStreak,
    int LongestStreak,
    DateOnly? LastStudyDate);
