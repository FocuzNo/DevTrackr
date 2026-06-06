namespace StatisticsService.Application.Statistics.Responses;

public sealed record TopicSummaryItemResponse(
    string Topic,
    int TotalMinutes);
