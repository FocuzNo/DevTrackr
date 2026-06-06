namespace StatisticsService.Application.Statistics.Responses;

public sealed record WeeklyStatisticsResponse(
    IReadOnlyList<DailyStatisticsItemResponse> Days,
    int TotalMinutes,
    int TotalSessions);
