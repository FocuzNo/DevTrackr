namespace StatisticsService.Application.Statistics.Responses;

public sealed record DashboardResponse(
    int TotalStudyMinutes,
    int TotalSessions,
    int CompletedGoals,
    int ActiveGoals,
    int CurrentStreak,
    int LongestStreak,
    decimal AverageDifficulty,
    IReadOnlyList<TopicSummaryItemResponse> TopTopics,
    IReadOnlyList<DailyStatisticsItemResponse> WeeklyMinutes);
