namespace StatisticsService.Application.Dashboard;

public sealed record DashboardResponse(
    Guid UserId,
    int TotalSessions,
    int TotalMinutes,
    int CompletedGoals,
    int CurrentStreakDays,
    IReadOnlyCollection<string> TopTopics);
