namespace StatisticsService.Application.Statistics.Responses;

public sealed record TopicStatisticsItemResponse(
    string Topic,
    int TotalMinutes,
    int SessionsCount,
    decimal AverageDifficulty);
