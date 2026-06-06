namespace StatisticsService.Application.Statistics.Responses;

public sealed record DailyStatisticsItemResponse(
    DateOnly Date,
    int TotalMinutes,
    int SessionsCount,
    decimal AverageDifficulty);
